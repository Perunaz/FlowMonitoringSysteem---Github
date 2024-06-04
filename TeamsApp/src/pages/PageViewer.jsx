import { useState, useEffect } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"
import FlowTable from "../components/FlowTable";
import FlowBarChart from "../components/FlowBarChart";

export function PageViewer(props) {
  const [selectedValue, setSelectedValue] = useState("dashboard");
  const [flowData, setFlowData] = useState([]);
  const [flowTableData, setflowTableData] = useState([]);

  const onTabSelect = (event, data) => {
    setSelectedValue(data.value);
  };
  
  useEffect(() => {
    fetch("http://localhost:7071/api/v1/flow")
      .then((response) => response.json())
      .then((data) => {
        const flowMap = {};

        data.forEach((item) => {
          const { FlowId, DisplayName, Status, SuccessPercentage, RunCount, Time } = item;
          // Check if flowMap already has the FlowId
          if (!flowMap[FlowId]) {
              flowMap[FlowId] = { name: '', data: [] };
          }
          // If DisplayName is different from the last stored name, update it
          if (flowMap[FlowId].name !== DisplayName) {
              flowMap[FlowId].name = DisplayName;
          }
          const timeOnly = new Date(Time).toLocaleTimeString("en-GB", {
              hour12: false,
          });
          flowMap[FlowId].data.push({
              time: timeOnly,
              status: Status,
              runCount: RunCount,
              successPercentage: SuccessPercentage,
              originalTime: Time // Storing original time for sorting
          });
      });

        Object.values(flowMap).forEach((flow) => {
          flow.data.sort((a, b) => new Date(a.originalTime) - new Date(b.originalTime));
        });

        setFlowData(Object.values(flowMap));

        // Extracting last status data
        const flowTableRows = Object.entries(flowMap).map(([flowId, flow]) => {
          return {
            flowId: flowId,
            name: flow.name,
            successPercentage: flow.data[flow.data.length - 1].successPercentage + "%",
            lastStatus: flow.data[flow.data.length - 1].status
          };
        });

        setflowTableData(flowTableRows);
      })
      .catch((error) => {
        console.error("Error fetching flow data:", error);
      });
  }, []);


  return (
    <div className="welcome page">
      <div className="narrow page-padding">
        <div className="tabList">
          <TabList selectedValue={selectedValue} onTabSelect={onTabSelect}>
            <Tab id="Dashboard" value="dashboard" className="tab">
              1. Dashboard
            </Tab>
            <Tab id="Top-runs" value="top-runs" className="tab">
              2. Top Runs
            </Tab>
          </TabList>
            {selectedValue === "dashboard" && (
              <>
                <div className="chart-container">
                  <FlowLineChart flows={flowData} />
                </div>
                <div>
                  <FlowTable rowData={flowTableData} />
                </div>
              </>
            )}
            {selectedValue === "top-runs" && (
              <>
                <div className="chart-container">
                  <p></p>
                  <FlowBarChart flows={flowData} />
                </div>
              </>
            )}
        </div>
      </div>
    </div>
  );
}
