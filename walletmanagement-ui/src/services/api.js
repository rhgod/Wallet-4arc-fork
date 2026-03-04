import axios from 'axios';

const BASE_API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5138/api';

const api = axios.create({
    baseURL: BASE_API_URL,
});


api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers = config.headers || {};
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);
//merhaba
export default api;