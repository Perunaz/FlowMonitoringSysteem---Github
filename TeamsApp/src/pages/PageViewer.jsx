import { useState } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"

export function PageViewer(props) {
  const [selectedValue, setSelectedValue] = useState("dashboard");

  const onTabSelect = (event, data) => {
    setSelectedValue(data.value);
  };
  return (
    <div className="welcome page">
      <div className="narrow page-padding">
        <div className="tabList">
          <TabList selectedValue={selectedValue} onTabSelect={onTabSelect}>
            <Tab id="Dashboard" value="dashboard">
              1. Dashboard
            </Tab>
            <Tab id="Dashboard1" value="dashboard1">
              2. Dashboard
            </Tab>
          </TabList>
          <div>
            {selectedValue === "dashboard" && (
              <FlowLineChart />
            )}
          </div>
          <div>
            {selectedValue === "dashboard1" && (
              <FlowLineChart />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
