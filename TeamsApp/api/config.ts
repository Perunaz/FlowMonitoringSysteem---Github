export interface Config {
    tenantId: string | undefined;
    clientId: string | undefined;
    clientSecret: string | undefined;
    redirectUri: string | undefined;
  }
  
  const config: Config = {
    tenantId: process.env.M365_TENANT_ID,
    clientId: process.env.API_CLIENT_ID,
    clientSecret: process.env.API_CLIENT_SECRET,
    redirectUri: process.env.REDIRECT_URI
  };
  
  export default config;