import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5244/api',
});

// Attach JWT Bearer token if present
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('eduspace_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const getStoredAuth = () => {
  try {
    const token = localStorage.getItem('eduspace_token');
    const user = localStorage.getItem('eduspace_user');
    return {
      token,
      user: user ? JSON.parse(user) : null,
    };
  } catch {
    return { token: null, user: null };
  }
};

export const setStoredAuth = (authData) => {
  if (authData?.token) {
    localStorage.setItem('eduspace_token', authData.token);
    localStorage.setItem(
      'eduspace_user',
      JSON.stringify({
        email: authData.email,
        roles: authData.roles || [],
      })
    );
  } else {
    localStorage.removeItem('eduspace_token');
    localStorage.removeItem('eduspace_user');
  }
};

export const loginUser = async (email, password) => {
  const response = await api.post('/auth/login', { email, password });
  if (response.data?.success && response.data?.token) {
    setStoredAuth(response.data);
  }
  return response.data;
};

export const logoutUser = () => {
  setStoredAuth(null);
};

export default api;
