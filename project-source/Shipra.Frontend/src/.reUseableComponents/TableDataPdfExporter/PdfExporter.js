// PdfExporter.jsx
import React, { forwardRef, useImperativeHandle } from "react";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";

const getImageBase64FromURL = (url) =>
  new Promise((resolve, reject) => {
    const img = new Image();
    img.crossOrigin = "Anonymous";
    img.onload = function () {
      const canvas = document.createElement("canvas");
      canvas.width = this.width;
      canvas.height = this.height;
      const ctx = canvas.getContext("2d");
      ctx.drawImage(this, 0, 0);
      try {
        const dataURL = canvas.toDataURL("image/png");
        resolve(dataURL);
      } catch (e) {
        reject(e);
      }
    };
    img.onerror = reject;
    img.src = url;
  });

const PdfExporter = forwardRef(
  (
    {
      columns = [],
      rows = [],
      fileName = "report.pdf",
      title = "Status Report",
      logoUrl = null,
      pageSize = "a4",
      orientation = "landscape",
      reportDate = null,
    },
    ref
  ) => {
    useImperativeHandle(ref, () => ({
      generatePDF,
      generateExcel,
    }));

    const addFooter = (doc) => {
      const pageCount = doc.internal.getNumberOfPages();
      for (let i = 1; i <= pageCount; i++) {
        doc.setPage(i);
        doc.setFontSize(10);
        doc.setFont("helvetica", "normal");
        doc.text(
          `Page ${i} of ${pageCount}`,
          doc.internal.pageSize.getWidth() - 30,
          doc.internal.pageSize.getHeight() - 10
        );
      }
    };

    const generatePDF = async () => {
      const reducedWidth = 225;
      const standardHeight = 297;

      const doc = new jsPDF({
        orientation: "portrait", // or "landscape" as needed
        unit: "mm",
        format: [reducedWidth, standardHeight],
      });
      doc.setFont("helvetica", "bold");

      let yOffset = 10;

      // Add logo
      if (logoUrl) {
        const logoWidth = 25;
        const logoHeight = 25 * 0.3;
        const pageWidth = doc.internal.pageSize.width;
        const marginRight = 10;
        const logoX = pageWidth - logoWidth - marginRight;
        try {
          const base64Logo = await getImageBase64FromURL(logoUrl);
          doc.addImage(base64Logo, "PNG", logoX, 5, logoWidth, logoHeight);
        } catch (err) {
          console.warn("Could not load logo:", err);
        }
      }

      // Title
      doc.setFontSize(18);
      const pageWidth = doc.internal.pageSize.getWidth();
      const textWidth = doc.getTextWidth(title);
      const titleX = (pageWidth - textWidth) / 2;
      doc.text(title, titleX, yOffset + 10);

      // Report date
      if (reportDate) {
        doc.setFontSize(12);
        doc.setFont("helvetica", "bold");
        const dateText = `Date: ${reportDate}`;
        const dateWidth = doc.getTextWidth(dateText);
        const dateX = (pageWidth - dateWidth) / 2;
        doc.text(dateText, dateX, yOffset + 20);
      }

      yOffset += 30;

      // Table headers and body
      const tableHead = columns.map((col) => col.exportLabel || col.field);
      const tableBody = rows.map((row) =>
        columns.map((col) => {
          const val = row[col.field];
          if (val === null || val === undefined) return "";
          if (typeof val === "boolean") return val ? "Yes" : "No";
          return String(val);
        })
      );

      let finalY = yOffset;

      autoTable(doc, {
        head: [tableHead],
        body: tableBody,
        startY: yOffset,
        styles: {
          font: "helvetica",
          fontSize: 8,
          cellPadding: 2,
          overflow: "linebreak",
          textColor: [0, 0, 0],
          lineColor: [0, 0, 0],
          lineWidth: 0.1,
        },
        headStyles: {
          fillColor: false,
          textColor: [0, 0, 0],
          fontSize: 9,
          fontStyle: "bold",
          lineColor: [0, 0, 0],
          lineWidth: 0.1,
        },
        theme: "grid",
        tableWidth: "auto",
        pageBreak: "auto",
        didDrawPage: (data) => {
          finalY = data.cursor.y;
        },
      });

      // Draw horizontal line (2px height with rounded edges)
      const lineMargin = 20;
      const lineStartX = lineMargin;
      const lineEndX = pageWidth - lineMargin;
      const lineY = finalY + 10;

      doc.setDrawColor(0, 0, 0);
      doc.setLineWidth(2);
      doc.roundedRect(lineStartX, lineY, lineEndX - lineStartX, 0, 2, 2, "F");

      addFooter(doc);
      doc.save(fileName);
    };

    const generateExcel = () => {
      const excelData = rows.map((row) => {
        const excelRow = {};
        columns.forEach((col) => {
          const val = row[col.field];
          const label =
            typeof col.headerName === "string"
              ? col.headerName
              : col.label || col.field;

          if (val === null || val === undefined) excelRow[label] = "";
          else if (typeof val === "boolean")
            excelRow[label] = val ? "Yes" : "No";
          else excelRow[label] = val;
        });
        return excelRow;
      });

      const worksheet = XLSX.utils.json_to_sheet(excelData);
      worksheet["!cols"] = columns.map((col) => ({
        wch: col.excelWidth || 20,
      }));

      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");
      XLSX.writeFile(workbook, fileName.replace(/\.pdf$/i, ".xlsx"));
    };

    return null;
  }
);

export default PdfExporter;
