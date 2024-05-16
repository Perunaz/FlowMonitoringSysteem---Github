import React from 'react';
import { mergeStyles } from '@fluentui/react/lib/Styling';

const tableStyle = mergeStyles({
  borderCollapse: 'collapse',
  width: '100%',
  border: '1px solid #CCCCCC',
});

const thStyle = mergeStyles({
  border: '1px solid #CCCCCC',
  padding: '8px',
  textAlign: 'left',
});

const tdStyle = mergeStyles({
  border: '1px solid #CCCCCC',
  padding: '8px',
  textAlign: 'left',
});

const Table = ({ rowData, columns }) => {
  if (!rowData || rowData.length === 0) {
    return <div>No data available</div>;
  }

  return (
    <table className={tableStyle}>
      <thead>
        <tr>
          {columns.map((column, index) => (
            <th key={index} className={thStyle}>{column.displayName}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rowData.map((row, rowIndex) => (
          <tr key={rowIndex}>
            {columns.map((column, colIndex) => (
              <td key={colIndex} className={tdStyle}>{row[column.key]}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default Table;