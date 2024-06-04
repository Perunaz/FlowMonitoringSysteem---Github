import React from 'react';
import './css/Table.css';

const Table = ({ rowData, columns }) => {
  if (!rowData || rowData.length === 0) {
    return <div>No data available</div>;
  }

  return (
    <table className="table">
      <thead>
        <tr>
          {columns.map((column, index) => (
            <th key={index} className="th">{column.displayName}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rowData.map((row, rowIndex) => (
          <tr key={rowIndex}>
            {columns.map((column, colIndex) => (
              <td key={colIndex} className="td">{row[column.key]}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default Table;