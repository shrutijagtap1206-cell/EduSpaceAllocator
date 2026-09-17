import api from './api';

export const getSpaces = () => api.get('/spaces').then(res => res.data);

export const createSpace = (space) =>
  api.post('/spaces', space).then(res => res.data);
