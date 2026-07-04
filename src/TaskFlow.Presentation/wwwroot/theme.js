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

window.downloadFile = function(base64, fileName) {
    const link = document.createElement('a');
    link.href = 'data:application/pdf;base64,' + base64;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
