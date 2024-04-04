import dotenv from 'dotenv';
import app from './app';

dotenv.config();

const port = process.env.PORT || 3100;

app.listen(port, () => {
  console.log(`server is listening on port ${port}`);
});
