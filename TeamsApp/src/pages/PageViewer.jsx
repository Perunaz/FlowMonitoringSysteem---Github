import { useState, useEffect } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"
import { PublicClientApplication } from '@azure/msal-browser';
import { checkIfTokenIsValid } from "../utils/checkIfTokenIsValid";
import config from "../components/config/config";

const msalConfig = {
  auth: {
    clientId: config.clientApiId,
    clientSecret: config.clientApiSecret,
    authority: `https://login.microsoftonline.com/${config.tenantApiId}/oauth2/authorize`,
    redirectUri: "https://localhost:53000/",
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
};

// Initialize MSAL instance
const msalInstance = new PublicClientApplication(msalConfig);

// Function to login and acquire token
async function loginAndGetToken() {
  // Check if a valid token is already stored
  const storedToken = sessionStorage.getItem("accessToken");

  if (checkIfTokenIsValid(storedToken)) {
    console.log("Token is valid, no need to log in again");
    return storedToken; // Return the stored token if it's valid
  }

  try {
    // Token is either missing or expired, initiate login
    const loginRequest = {
      scopes: ['https://service.flow.microsoft.com//.default', 'offline_access'],
    };

    const loginResult = await msalInstance.loginPopup(loginRequest);

    // Store new access token in sessionStorage
    const accessToken = loginResult.accessToken;
    const refreshToken = loginResult.refreshToken;
    sessionStorage.setItem("accessToken", accessToken);
    sessionStorage.setItem("refreshToken", refreshToken);

    console.log("New token obtained");
    return accessToken; // Return the new token
  } catch (error) {
    console.error("Error during login:", error);
  }
}

export function PageViewer(props) {
  const [selectedValue, setSelectedValue] = useState("dashboard");
  const [initialized, setInitialized] = useState(false);
  const [flowData, setFlowData] = useState([]);

  const onTabSelect = (event, data) => {
    setSelectedValue(data.value);
  };

  useEffect(() => {
    console.log(config)
    const initializeMsal = async () => {
      msalInstance.initialize();
      setInitialized(true);
    };

    initializeMsal();
  }, []);

  useEffect(() => {
    if (initialized) {
      loginAndGetToken();
    }
    }, [initialized]);

  // useEffect(() => { 
  //   fetch("http://localhost:7071/api/v1/flow")
  //     .then((response) => response.json())
  //     .then((data) => {
  //       // Group data by FlowId and process into the desired structure
  //       const flowMap = {};
  
  //       data.forEach((item) => {
  //         const { FlowId, StateCode, Time } = item;
  //         if (!flowMap[FlowId]) {
  //           flowMap[FlowId] = { name: FlowId, data: [] };
  //         }
  //         const timeOnly = new Date(Time).toLocaleTimeString("en-GB", {
  //           hour12: false,
  //         });
  //         flowMap[FlowId].data.push({
  //           time: timeOnly,
  //           statecode: StateCode,
  //           originalTime: Time,
  //         });
  //       });
  
  //       // Sort the 'data' arrays within each flowMap entry by 'originalTime'
  //       Object.values(flowMap).forEach((flow) => {
  //         flow.data.sort((a, b) => new Date(a.originalTime) - new Date(b.originalTime));
  //       });
  
  //       // Convert the map to an array and set the sorted data
  //       setFlowData(Object.values(flowMap));
  //     })
  //     .catch((error) => {
  //       console.error("Error fetching flow data:", error);
  //     });
  // }, []);

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
