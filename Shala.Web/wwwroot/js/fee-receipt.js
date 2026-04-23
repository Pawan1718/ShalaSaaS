window.feeReceipt = {
    printElement: function (id) {
        const el = document.getElementById(id);
        if (!el) return;

        const printWindow = window.open('', '_blank', 'width=900,height=1200');
        if (!printWindow) return;

        printWindow.document.write(`
            <html>
            <head>
                <title>Fee Receipt</title>
                <link rel="stylesheet" href="/fee-receipt-print.css" />
                <style>
                    body {
                        margin: 0;
                        padding: 20px;
                        background: white;
                        font-family: Arial, sans-serif;
                    }
                </style>
            </head>
            <body>
                ${el.outerHTML}
            </body>
            </html>
        `);

        printWindow.document.close();
        printWindow.focus();

        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 500);
    },

    downloadPdf: function (id) {
        window.feeReceipt.printElement(id);
    }
};