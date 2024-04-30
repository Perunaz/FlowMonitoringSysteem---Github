import { useState, useEffect } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"

export function PageViewer(props) {
  const [selectedValue, setSelectedValue] = useState("dashboard");
  const [flowData, setFlowData] = useState([]);

  const onTabSelect = (event, data) => {
    setSelectedValue(data.value);
  };
  useEffect(() => {
    fetch("http://localhost:7071/api/v1/flow")
      .then((response) => response.json())
      .then((data) => {
        // Group data by FlowId and process into the desired structure
        const flowMap = {};
  
        data.forEach((item) => {
          const { FlowId, StateCode, Time } = item;
          if (!flowMap[FlowId]) {
            flowMap[FlowId] = { name: FlowId, data: [] };
          }
          const timeOnly = new Date(Time).toLocaleTimeString("en-GB", {
            hour12: false,
          });
          flowMap[FlowId].data.push({
            time: timeOnly,
            statecode: StateCode,
            originalTime: Time,
          });
        });
  
        // Sort the 'data' arrays within each flowMap entry by 'originalTime'
        Object.values(flowMap).forEach((flow) => {
          flow.data.sort((a, b) => new Date(a.originalTime) - new Date(b.originalTime));
        });
  
        // Convert the map to an array and set the sorted data
        setFlowData(Object.values(flowMap));
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
            <Tab id="Dashboard" value="dashboard">
              1. Dashboard
            </Tab>
          </TabList>
          <div className="chart-container">
            {selectedValue === "dashboard" && (
              <FlowLineChart flows={flowData} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
