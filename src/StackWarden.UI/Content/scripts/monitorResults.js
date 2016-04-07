stackwarden.monitorResults = {
    initialize: function (options) {
        var loadedOptions = $.extend(stackwarden.monitorResults.defaultOptions(),
                                     options);
        var monitorResults = new stackwarden.monitorResults.models.monitorResultList(loadedOptions);

        loadedOptions.initialData.forEach(function (currentResult) {
            monitorResults.results.push(new stackwarden.monitorResults.models.monitorResult(currentResult, loadedOptions));
        });

        var $notificationHub = $.connection.notificationHub;
        $notificationHub.client.addMonitorResult = function (newResult) {
            var modelResult = new stackwarden.monitorResults.models.monitorResult(newResult, loadedOptions);
            var didFind = false;

            monitorResults.results().forEach(function (item, index) {
                if (item.name() === modelResult.name()) {
                    didFind = true;

                    item.name(modelResult.name());
                    item.details(modelResult.details());
                    item.targetName(modelResult.targetName());
                    item.staleAfter(modelResult.staleAfter());
                    item.state(modelResult.state());
                    item.message(modelResult.message());
                    item.icon(modelResult.icon());
                }
            });

            if (!didFind)
                monitorResults.results.push(modelResult);

            stackwarden.monitorResults.sortResults(monitorResults.results, loadedOptions);
        };

        ko.applyBindings(monitorResults);

        $.connection.hub.start();
    },
    defaultOptions: function () {
        return {
            normal: 'normal',
            warning: 'warning',
            error: 'error',
            initialData: []      
        };
    },
    models: {
        monitorResult: function (source, options) {
            var self = this;
            var loadedOptions = $.extend(stackwarden.monitorResults.defaultOptions(),
                                         options);

            self.name = ko.observable(source.Name);
            self.details = ko.observableArray(stackwarden.utilities.convertToKeyValuePairs(source.Details));
            self.hasDetails = ko.pureComputed(function () {
                return self.details() !== null && self.details().length > 0;
            });
            self.message = ko.observable(source.Message);
            self.targetName = ko.observable(source.TargetName);
            self.staleAfter = ko.observable(new Date(source.StaleAfter));
            self.state = ko.observable(source.State);
            self.icon = ko.observable(source.Icon);
            self.tags = ko.observableArray(source.Tags);
            self.groupKey = ko.computed(function () {
                return self.tags().sort().join('');
            });
            self.lastUIRefresh = ko.observable(new Date());
            self.isStale = ko.pureComputed(function () {
                return self.staleAfter() < self.lastUIRefresh();
            }, self);
            self.shouldShowMessage = ko.pureComputed(function () {
                return self.state() != loadedOptions.normal;
            }, self);
            self.shouldShowControlPanel = ko.observable(false);
            self.toggleControlPanel = function () {
                self.shouldShowControlPanel(!self.shouldShowControlPanel());
            };
            self.shouldShowDetailsPanel = ko.observable(true);
            self.toggleDetailsPanel = function () {
                self.shouldShowDetailsPanel(!self.shouldShowDetailsPanel());

                if (self.shouldShowDetailsPanel())
                    self.shouldShowToolsPanel(false);
            };
            self.shouldShowToolsPanel = ko.observable(false);
            self.toggleToolsPanel = function () {
                self.shouldShowToolsPanel(!self.shouldShowToolsPanel());

                if (self.shouldShowToolsPanel())
                    self.shouldShowDetailsPanel(false);
            };
            self.tools = ko.observable(source.Tools);
            self.hasTools = ko.pureComputed(function () {
                return self.tools() !== null &&
                        self.tools().length > 0;
            });

            self.staleAfter.subscribe(function (newValue) {
                setTimeout(function () {
                    self.lastUIRefresh(new Date());
                }, newValue - new Date() + 100);
            });
        },
        monitorResultGroup: function (key, options) {
            var self = this;
            var loadedOptions = $.extend(stackwarden.monitorResults.defaultOptions(),
                                         options);

            self.groupKey = ko.observable(key);
            self.results = ko.observableArray();
            self.state = ko.computed(function () {
                return self.results().length > 0
                        ? self.results()[0].state()
                        : loadedOptions.normal;
            });
            self.selectedResult = ko.observable();
            self.select = function (clickedResult) {
                self.selectedResult(self.selectedResult() !== clickedResult
                                        ? clickedResult
                                        : null);
            };
            self.shouldShowControlPanel = ko.computed(function () {
                return self.selectedResult() !== null;
            });
            self.shouldShowMessage = ko.pureComputed(function () {
                return self.state() != loadedOptions.normal;
            });
            self.message = ko.computed(function () {
                var combinedMessage = "";

                self.results().forEach(function (currentResult) {
                    if (currentResult.state() !== loadedOptions.normal && currentResult.message())
                        combinedMessage += currentResult.message() + "\n";
                });

                return combinedMessage;
            });
        },
        monitorResultList: function (options) {
            var self = this;
            var loadedOptions = $.extend(stackwarden.monitorResults.defaultOptions(),
                                         options);

            self.results = ko.observableArray();
            self.groups = ko.observableArray();

            self.results.subscribe(function (newValues) {
                newValues.forEach(function (newValue) {
                    var targetGroup = null;

                    self.groups().forEach(function (currentGroup) {
                        if (currentGroup.groupKey() == newValue.groupKey()) {
                            targetGroup = currentGroup;
                        }
                    });

                    if (targetGroup == null) {
                        targetGroup = new stackwarden.monitorResults.models.monitorResultGroup(newValue.groupKey(), loadedOptions);
                        self.groups.push(targetGroup);
                    }

                    var wasFound = false;

                    targetGroup.results().forEach(function (currentResult) {
                        if (currentResult.name() == newValue.name()) {
                            wasFound = true;
                        }
                    });

                    if (wasFound === false) {
                        targetGroup.results.push(newValue);
                        stackwarden.monitorResults.sortResults(targetGroup.results, options);
                    }
                });
            });
        }
    },
    sortResults: function(results, options) {
        var stateWeight = {};
        var loadedOptions = $.extend(stackwarden.monitorResults.defaultOptions(),
                                     options);
        stateWeight[loadedOptions.normal] = 0;
        stateWeight[loadedOptions.warning] = 1;
        stateWeight[loadedOptions.error] = 2;

        results.sort(function (left, right) {
            var leftState = stateWeight[left.state()];
            var rightState = stateWeight[right.state()];
            var stateComparisonResult = leftState < rightState
                                            ? 1
                                            : rightState < leftState
                                                ? -1
                                                : 0;

            if (stateComparisonResult !== 0)
                return stateComparisonResult;

            var nameComparisonResult = left.name().localeCompare(right.name());

            return nameComparisonResult;
        });
    }
};