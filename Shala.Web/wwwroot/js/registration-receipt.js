window.registrationReceipt = {
    printElement: function (id) {
        window.print();
    },

    downloadPdf: function (id) {
        // Browser print dialog opens.
        // User can choose "Save as PDF".
        window.print();
    },

    sendWhatsApp: function (phoneNumber, message) {
        if (!phoneNumber) return;

        const cleanNumber = phoneNumber.replace(/[^\d]/g, "");
        const encodedMessage = encodeURIComponent(message || "");
        const url = `https://wa.me/${cleanNumber}?text=${encodedMessage}`;

        window.open(url, "_blank");
    }
};