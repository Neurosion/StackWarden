var stackwarden = {
    utilities: {
        convertToKeyValuePairs: function (source) {
            var pairs = [];

            for (var key in source)
                if (source.hasOwnProperty(key))
                    pairs.push({ key: key, value: source[key] });

            return pairs;
        }
    }
};