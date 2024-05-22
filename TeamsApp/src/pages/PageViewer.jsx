import { useState, useEffect } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"
import FlowTable from "../components/FlowTable";
import FlowBarChart from "../components/FlowBarChart";
//import { PublicClientApplication } from '@azure/msal-browser';
//import { checkIfTokenIsValid } from "../utils/checkIfTokenIsValid";
//import config from "../components/config/config";

// const msalConfig = {
//   auth: {
//     clientId: config.clientApiId,
//     clientSecret: config.clientApiSecret,
//     authority: `https://login.microsoftonline.com/${config.tenantApiId}/oauth2/authorize`,
//     redirectUri: "https://localhost:53000/",
//   },
//   cache: {
//     cacheLocation: 'sessionStorage',
//   },
// };

// // Initialize MSAL instance
// const msalInstance = new PublicClientApplication(msalConfig);

// // Function to login and acquire token
// async function loginAndGetToken() {
//   // Check if a valid token is already stored
//   const storedToken = sessionStorage.getItem("accessToken");

//   if (checkIfTokenIsValid(storedToken)) {
//     console.log("Token is valid, no need to log in again");
//     return storedToken; // Return the stored token if it's valid
//   }

//   try {
//     // Token is either missing or expired, initiate login
//     const loginRequest = {
//       scopes: ['https://service.flow.microsoft.com//.default', 'offline_access'],
//     };

//     const loginResult = await msalInstance.loginPopup(loginRequest);

//     // Store new access token in sessionStorage
//     const accessToken = loginResult.accessToken;
//     sessionStorage.setItem("accessToken", accessToken);

//     fetch("http://localhost:7071/api/v1/token", {
//       method: "POST",
//       headers: {
//         "Content-Type": "application/json",
//       },
//       body: JSON.stringify(accessToken),
//     })
//       .then((response) => {
//         if (!response.ok) {
//           throw new Error("Network response was not ok");
//         }
//         return response.json(); // Parse the JSON response
//       })
//       .then((data) => {
//         // Handle the response data (if any)
//         console.log("Received response:", data);
//       })
//       .catch((error) => {
//         // Handle errors
//         console.error("Error in POST request:", error);
//       });

//     return accessToken; // Return the new token
//   } catch (error) {
//     console.error("Error during login:", error);
//   }
// }

export function PageViewer(props) {
  const [selectedValue, setSelectedValue] = useState("dashboard");
//  const [initialized, setInitialized] = useState(false);
  const [flowData, setFlowData] = useState([]);
  const [flowTableData, setflowTableData] = useState([]);

  const onTabSelect = (event, data) => {
    setSelectedValue(data.value);
  };

  // useEffect(() => {
  //   console.log(config)
  //   const initializeMsal = async () => {
  //     msalInstance.initialize();
  //     setInitialized(true);
  //   };

  //   initializeMsal();
  // }, []);

  // useEffect(() => {
  //   if (initialized) {
  //     loginAndGetToken();
  //   }
  //   }, [initialized]);

  // useEffect(() => {
  //   fetch('http://localhost:7071/api/v1/login', {
  //     method: 'GET',
  //     headers: {
  //       'Access-Control-Allow-Origin': '*' 
  //     },
  //     redirect: 'follow', // Ensure the response follows any redirection
  //   })
  //     .then((response) => {
  //       if (response.redirected) {
  //         // If the response contains a redirection, navigate to the new URL
  //         window.location.href = response.url;
  //       }
  //     })
  //     .catch((error) => {
  //       console.error('Error initiating login:', error);
  //     });
  // }, []);
  
  useEffect(() => {
    fetch("http://localhost:7071/api/v1/flow")
      .then((response) => response.json())
      .then((data) => {
        const flowMap = {};
        console.log(data);

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
        const flowTableRows = Object.values(flowMap).map(flow => {
          return {
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
