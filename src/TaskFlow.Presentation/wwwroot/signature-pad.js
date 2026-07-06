window.signaturePad = {
    instances: {},

    init: function (id) {
        const canvas = document.getElementById(id);
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        let drawing = false;
        let lastX = 0, lastY = 0;

        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width;
        canvas.height = rect.height;

        ctx.strokeStyle = '#000';
        ctx.lineWidth = 2;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';

        function getPos(e) {
            const rect = canvas.getBoundingClientRect();
            const clientX = e.touches ? e.touches[0].clientX : e.clientX;
            const clientY = e.touches ? e.touches[0].clientY : e.clientY;
            return { x: clientX - rect.left, y: clientY - rect.top };
        }

        function startDrawing(e) {
            e.preventDefault();
            drawing = true;
            const pos = getPos(e);
            lastX = pos.x;
            lastY = pos.y;
        }

        function draw(e) {
            e.preventDefault();
            if (!drawing) return;
            const pos = getPos(e);
            ctx.beginPath();
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(pos.x, pos.y);
            ctx.stroke();
            lastX = pos.x;
            lastY = pos.y;
        }

        function stopDrawing() {
            drawing = false;
        }

        canvas.addEventListener('mousedown', startDrawing);
        canvas.addEventListener('mousemove', draw);
        canvas.addEventListener('mouseup', stopDrawing);
        canvas.addEventListener('mouseleave', stopDrawing);
        canvas.addEventListener('touchstart', startDrawing, { passive: false });
        canvas.addEventListener('touchmove', draw, { passive: false });
        canvas.addEventListener('touchend', stopDrawing);

        this.instances[id] = { canvas, ctx };
    },

    getData: function (id) {
        const canvas = document.getElementById(id);
        if (!canvas) return null;
        return canvas.toDataURL('image/png');
    },

    clear: function (id) {
        const canvas = document.getElementById(id);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
    },

    resize: function (id) {
        const canvas = document.getElementById(id);
        if (!canvas) return;
        const data = canvas.toDataURL();
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width;
        canvas.height = rect.height;
        const ctx = canvas.getContext('2d');
        ctx.strokeStyle = '#000';
        ctx.lineWidth = 2;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        const img = new Image();
        img.onload = function () {
            ctx.drawImage(img, 0, 0);
        };
        img.src = data;
    }
};
