import React from 'react';
import './css/FlowTable.css';
import config from "./config/config";

const getBackgroundColor = (successPercentageString) => {
  const successPercentage = parseInt(successPercentageString.replace('%', ''), 10);
  const hue = (successPercentage / 100) * 120;
  return `hsl(${hue}, 70%, 60%)`;
};

const getStatusBackgroundColor = (status) => {
  switch (status) {
    case 'Failed':
      return '#e05252';
    case 'Succeeded':
      return '#52e052';
    default:
      return ''; // Default background color
  }
};

const FlowTable = ({ rowData }) => {
  if (!rowData || rowData.length === 0) {
    return <div>No data available</div>;
  }
  
  return (
    <div className="flow-table">
      <table className="table">
        <thead>
          <tr>
            <th className="th success-percentage">Success %</th>
            <th className="th flow-name">Flow Name</th>
            <th className="th last-status">Last Status</th>
          </tr>
        </thead>
        <tbody>
          {rowData.map((row, rowIndex) => {
            const flowUrl = `https://make.powerautomate.com/environments/Default-${config.tenantApiId}/flows/${row.flowId}/details`;
            return (
              <tr key={rowIndex}>
                <td className="td success-percentage" style={{ backgroundColor: getBackgroundColor(row.successPercentage) }}>{row.successPercentage}</td>
                <td className="td flow-name">
                  <a href={flowUrl} target="_blank" rel="noopener noreferrer">{row.name}</a>
                </td>
                <td className="td last-status" style={{ backgroundColor: getStatusBackgroundColor(row.lastStatus) }}>{row.lastStatus}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default FlowTable;
