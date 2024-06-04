
import dotenv from 'dotenv';
import app from './app';

dotenv.config();

const port = process.env.PORT || 7071;

app.listen(port, () => {
  console.log(`Server is listening on port ${port}`);
});

