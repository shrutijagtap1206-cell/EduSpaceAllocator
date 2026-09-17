import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5244/api',
});

export default api;
