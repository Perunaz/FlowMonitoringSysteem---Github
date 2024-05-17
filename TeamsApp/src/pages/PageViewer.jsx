import { useState, useEffect } from "react";
import { TabList, Tab } from "@fluentui/react-components";
import "./css/PageViewer.css";
import FlowLineChart from "../components/FlowLineChart"
import Table from "../components/Table";
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
  const [lastStatusData, setLastStatusData] = useState([]);

  const columns = [
    { key: 'name', displayName: 'Flow Name' },
    { key: 'lastStatus', displayName: 'Last Status' },
  ];

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

        data.forEach((item) => {
          const { FlowId, DisplayName, Status, Time } = item;
          if (!flowMap[FlowId]) {
            flowMap[FlowId] = { name: DisplayName, data: [] };
          }
          const timeOnly = new Date(Time).toLocaleTimeString("en-GB", {
            hour12: false,
          });
          flowMap[FlowId].data.push({
            time: timeOnly,
            status: Status,
            originalTime: Time // Storing original time for sorting
          });
        });

        Object.values(flowMap).forEach((flow) => {
          flow.data.sort((a, b) => new Date(a.originalTime) - new Date(b.originalTime));
        });

        setFlowData(Object.values(flowMap));

        // Extracting last status data
        const lastStatus = Object.values(flowMap).map(flow => ({
          name: flow.name,
          lastStatus: flow.data[flow.data.length - 1].status
        }));

        setLastStatusData(lastStatus);
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
            {selectedValue === "dashboard" && (
              <>
                <div className="chart-container">
                  <FlowLineChart flows={flowData} />
                </div>
                <div>
                  <Table rowData={lastStatusData} columns={columns} />
                </div>
              </>
            )}
        </div>
      </div>
    </div>
  );
}
