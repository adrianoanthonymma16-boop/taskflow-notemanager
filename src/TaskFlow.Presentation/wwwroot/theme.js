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
    const byteChars = atob(base64);
    const byteNums = new Array(byteChars.length);
    for (let i = 0; i < byteChars.length; i++) byteNums[i] = byteChars.charCodeAt(i);
    const byteArray = new Uint8Array(byteNums);
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const blobUrl = URL.createObjectURL(blob);

    window.open(blobUrl, '_blank');

    const link = document.createElement('a');
    link.href = blobUrl;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    setTimeout(() => URL.revokeObjectURL(blobUrl), 10000);
};
