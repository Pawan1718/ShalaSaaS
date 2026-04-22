window.registrationReceipt = {
    printPage: function () {
        window.print();
    },

    downloadPdf: function (fileName) {
        const originalTitle = document.title;
        document.title = fileName || "receipt";

        window.print();

        setTimeout(() => {
            document.title = originalTitle;
        }, 500);
    },

    sendWhatsApp: function (phoneNumber, message) {
        if (!phoneNumber) return;

        const cleanNumber = phoneNumber.replace(/[^\d]/g, "");
        const encodedMessage = encodeURIComponent(message || "");
        const url = `https://wa.me/${cleanNumber}?text=${encodedMessage}`;

        window.open(url, "_blank");
    }
};