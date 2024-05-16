// Function to check if token is valid
import { jwtDecode } from "jwt-decode";

export const checkIfTokenIsValid = (token) => {
    if (!token) {
      return false;
    }
  
    try {
      const decodedToken = jwtDecode(token);
      const currentTime = Date.now() / 1000; // Convert to seconds
      return decodedToken.exp > currentTime; // Check if the token is still valid
    } catch (e) {
      console.error("Invalid token", e);
      return false;
    }
  }