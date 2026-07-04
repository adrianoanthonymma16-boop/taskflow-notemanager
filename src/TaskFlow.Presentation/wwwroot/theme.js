window.themeManager = {
    isDark: localStorage.getItem('taskflow-dark') === 'true',

    init: function() {
        if (this.isDark) {
            document.documentElement.classList.add('dark-mode');
        }
    },

    toggle: function() {
        this.isDark = !this.isDark;
        localStorage.setItem('taskflow-dark', this.isDark);
        if (this.isDark) {
            document.documentElement.classList.add('dark-mode');
        } else {
            document.documentElement.classList.remove('dark-mode');
        }
        return this.isDark;
    }
};

themeManager.init();
