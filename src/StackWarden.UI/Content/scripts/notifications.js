stackwarden.notifications = {
    notify: function (title, message, icon, autoCloseAfter) {
        if (!stackwarden.notifications.isEnabled())
            return false;

        var shouldAutoClose = autoCloseAfter !== undefined;

        var options = {
            body: message,
            requireInteraction: !shouldAutoClose
        };

        if (icon !== undefined)
            options.icon = icon;

        var prefixedTitle = "StackWarden - " + title;
        var notification = new Notification(prefixedTitle, options);

        if (shouldAutoClose) {
            setTimeout(notification.close.bind(notification), autoCloseAfter);
        }

        return true;
    },
    isSupported: function () {
        return "Notification" in window;
    },
    isEnabled: function() {
        return stackwarden.notifications.isSupported() &&
               Notification.permission === "granted";
    },
    enable: function() {
        if (Notification.permission !== "default")
            return;

        Notification.requestPermission();
    }
};