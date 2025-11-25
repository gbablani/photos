import axios from 'axios';
import type {
  AuthProvider,
  AuthResponse,
  User,
  Photo,
  PhotoListResponse,
  PhotoSearch,
  Album,
  AlbumWithPhotos,
  EnhancementJob,
  JobType,
  JobStatus,
  SubscriptionStatus,
  AnimationStyle,
  ExternalPhotosResponse,
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  externalAuth: async (provider: AuthProvider, accessToken: string): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/external', {
      provider,
      accessToken,
    });
    return response.data;
  },

  getCurrentUser: async (): Promise<User> => {
    const response = await api.get<User>('/auth/me');
    return response.data;
  },

  updateProfile: async (update: { displayName?: string; autoSyncEnabled?: boolean }): Promise<User> => {
    const response = await api.patch<User>('/auth/me', update);
    return response.data;
  },
};

// Photos API
export const photosApi = {
  getPhotos: async (search?: PhotoSearch): Promise<PhotoListResponse> => {
    const response = await api.get<PhotoListResponse>('/photos', { params: search });
    return response.data;
  },

  getPhoto: async (id: string): Promise<Photo> => {
    const response = await api.get<Photo>(`/photos/${id}`);
    return response.data;
  },

  uploadPhoto: async (file: File, metadata?: { description?: string; tags?: string }): Promise<Photo> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('FileName', file.name);
    formData.append('ContentType', file.type);
    if (metadata?.description) formData.append('Description', metadata.description);
    if (metadata?.tags) formData.append('Tags', metadata.tags);

    const response = await api.post<Photo>('/photos', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  updatePhoto: async (id: string, update: { description?: string; tags?: string }): Promise<Photo> => {
    const response = await api.patch<Photo>(`/photos/${id}`, update);
    return response.data;
  },

  deletePhoto: async (id: string): Promise<void> => {
    await api.delete(`/photos/${id}`);
  },

  getPhotosByPerson: async (personName: string): Promise<Photo[]> => {
    const response = await api.get<Photo[]>(`/photos/by-person/${encodeURIComponent(personName)}`);
    return response.data;
  },

  getPhotosByDateRange: async (startDate: string, endDate: string): Promise<Photo[]> => {
    const response = await api.get<Photo[]>('/photos/by-date', {
      params: { startDate, endDate },
    });
    return response.data;
  },
};

// Albums API
export const albumsApi = {
  getAlbums: async (): Promise<Album[]> => {
    const response = await api.get<Album[]>('/albums');
    return response.data;
  },

  getAlbum: async (id: string): Promise<AlbumWithPhotos> => {
    const response = await api.get<AlbumWithPhotos>(`/albums/${id}`);
    return response.data;
  },

  createAlbum: async (data: { name: string; description?: string }): Promise<Album> => {
    const response = await api.post<Album>('/albums', data);
    return response.data;
  },

  updateAlbum: async (id: string, data: { name: string; description?: string }): Promise<Album> => {
    const response = await api.patch<Album>(`/albums/${id}`, data);
    return response.data;
  },

  deleteAlbum: async (id: string): Promise<void> => {
    await api.delete(`/albums/${id}`);
  },

  addPhotosToAlbum: async (albumId: string, photoIds: string[]): Promise<void> => {
    await api.post(`/albums/${albumId}/photos`, { photoIds });
  },

  removePhotoFromAlbum: async (albumId: string, photoId: string): Promise<void> => {
    await api.delete(`/albums/${albumId}/photos/${photoId}`);
  },
};

// Enhancements API
export const enhancementsApi = {
  getSubscriptionStatus: async (): Promise<SubscriptionStatus> => {
    const response = await api.get<SubscriptionStatus>('/enhancements/subscription');
    return response.data;
  },

  purchaseCredits: async (creditPackage: number): Promise<void> => {
    await api.post('/enhancements/purchase-credits', { creditPackage });
  },

  subscribe: async (tier: 'Premium'): Promise<void> => {
    await api.post('/enhancements/subscribe', { tier });
  },

  createJob: async (data: {
    jobType: JobType;
    sourcePhotoId?: string;
    sourceVideoId?: string;
    additionalPhotoIds?: string[];
    options?: {
      animationStyle?: AnimationStyle;
      addMusic?: boolean;
      durationSeconds?: number;
      personPhotoId?: string;
    };
  }): Promise<EnhancementJob> => {
    const response = await api.post<EnhancementJob>('/enhancements/jobs', data);
    return response.data;
  },

  getJob: async (id: string): Promise<EnhancementJob> => {
    const response = await api.get<EnhancementJob>(`/enhancements/jobs/${id}`);
    return response.data;
  },

  getJobs: async (status?: JobStatus): Promise<EnhancementJob[]> => {
    const response = await api.get<EnhancementJob[]>('/enhancements/jobs', { params: { status } });
    return response.data;
  },

  colorizePhoto: async (photoId: string): Promise<EnhancementJob> => {
    const response = await api.post<EnhancementJob>(`/enhancements/colorize/${photoId}`);
    return response.data;
  },

  restorePhoto: async (photoId: string): Promise<EnhancementJob> => {
    const response = await api.post<EnhancementJob>(`/enhancements/restore/${photoId}`);
    return response.data;
  },

  animatePhoto: async (
    photoId: string,
    options?: { style?: AnimationStyle; addMusic?: boolean; durationSeconds?: number }
  ): Promise<EnhancementJob> => {
    const response = await api.post<EnhancementJob>(`/enhancements/animate/${photoId}`, options || {});
    return response.data;
  },

  createMontage: async (data: {
    photoIds: string[];
    style?: AnimationStyle;
    addMusic?: boolean;
    durationSeconds?: number;
  }): Promise<EnhancementJob> => {
    const response = await api.post<EnhancementJob>('/enhancements/montage', data);
    return response.data;
  },
};

// Integrations API
export const integrationsApi = {
  connectGooglePhotos: async (authorizationCode: string, redirectUri?: string): Promise<void> => {
    await api.post('/integrations/google-photos/connect', { authorizationCode, redirectUri });
  },

  disconnectGooglePhotos: async (): Promise<void> => {
    await api.post('/integrations/google-photos/disconnect');
  },

  browseGooglePhotos: async (pageToken?: string, pageSize?: number): Promise<ExternalPhotosResponse> => {
    const response = await api.get<ExternalPhotosResponse>('/integrations/google-photos/browse', {
      params: { pageToken, pageSize },
    });
    return response.data;
  },

  importFromGooglePhotos: async (externalIds: string[]): Promise<void> => {
    await api.post('/integrations/google-photos/import', { externalIds });
  },

  exportToGooglePhotos: async (photoId: string): Promise<void> => {
    await api.post(`/integrations/google-photos/export/${photoId}`);
  },

  connectOneDrive: async (authorizationCode: string, redirectUri?: string): Promise<void> => {
    await api.post('/integrations/onedrive/connect', { authorizationCode, redirectUri });
  },

  disconnectOneDrive: async (): Promise<void> => {
    await api.post('/integrations/onedrive/disconnect');
  },

  browseOneDrive: async (path?: string, pageToken?: string, pageSize?: number): Promise<ExternalPhotosResponse> => {
    const response = await api.get<ExternalPhotosResponse>('/integrations/onedrive/browse', {
      params: { path, pageToken, pageSize },
    });
    return response.data;
  },

  importFromOneDrive: async (externalIds: string[]): Promise<void> => {
    await api.post('/integrations/onedrive/import', { externalIds });
  },

  exportToOneDrive: async (photoId: string, targetPath?: string): Promise<void> => {
    await api.post(`/integrations/onedrive/export/${photoId}`, null, { params: { targetPath } });
  },
};

export default api;
