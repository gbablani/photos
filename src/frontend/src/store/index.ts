import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, Photo, Album, EnhancementJob, SubscriptionStatus } from '../types';

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  setAuth: (user: User, accessToken: string, refreshToken: string) => void;
  logout: () => void;
  updateUser: (user: Partial<User>) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      setAuth: (user, accessToken, refreshToken) => {
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        set({ user, accessToken, refreshToken, isAuthenticated: true });
      },
      logout: () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        set({ user: null, accessToken: null, refreshToken: null, isAuthenticated: false });
      },
      updateUser: (updates) =>
        set((state) => ({
          user: state.user ? { ...state.user, ...updates } : null,
        })),
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ user: state.user, isAuthenticated: state.isAuthenticated }),
    }
  )
);

interface PhotosState {
  photos: Photo[];
  selectedPhotos: string[];
  currentPhoto: Photo | null;
  totalCount: number;
  page: number;
  pageSize: number;
  setPhotos: (photos: Photo[], totalCount: number, page: number, pageSize: number) => void;
  addPhoto: (photo: Photo) => void;
  updatePhoto: (id: string, updates: Partial<Photo>) => void;
  removePhoto: (id: string) => void;
  setCurrentPhoto: (photo: Photo | null) => void;
  toggleSelectPhoto: (id: string) => void;
  selectAllPhotos: () => void;
  clearSelection: () => void;
}

export const usePhotosStore = create<PhotosState>((set) => ({
  photos: [],
  selectedPhotos: [],
  currentPhoto: null,
  totalCount: 0,
  page: 1,
  pageSize: 20,
  setPhotos: (photos, totalCount, page, pageSize) => set({ photos, totalCount, page, pageSize }),
  addPhoto: (photo) => set((state) => ({ photos: [photo, ...state.photos] })),
  updatePhoto: (id, updates) =>
    set((state) => ({
      photos: state.photos.map((p) => (p.id === id ? { ...p, ...updates } : p)),
      currentPhoto: state.currentPhoto?.id === id ? { ...state.currentPhoto, ...updates } : state.currentPhoto,
    })),
  removePhoto: (id) =>
    set((state) => ({
      photos: state.photos.filter((p) => p.id !== id),
      selectedPhotos: state.selectedPhotos.filter((pid) => pid !== id),
      currentPhoto: state.currentPhoto?.id === id ? null : state.currentPhoto,
    })),
  setCurrentPhoto: (photo) => set({ currentPhoto: photo }),
  toggleSelectPhoto: (id) =>
    set((state) => ({
      selectedPhotos: state.selectedPhotos.includes(id)
        ? state.selectedPhotos.filter((pid) => pid !== id)
        : [...state.selectedPhotos, id],
    })),
  selectAllPhotos: () => set((state) => ({ selectedPhotos: state.photos.map((p) => p.id) })),
  clearSelection: () => set({ selectedPhotos: [] }),
}));

interface AlbumsState {
  albums: Album[];
  setAlbums: (albums: Album[]) => void;
  addAlbum: (album: Album) => void;
  updateAlbum: (id: string, updates: Partial<Album>) => void;
  removeAlbum: (id: string) => void;
}

export const useAlbumsStore = create<AlbumsState>((set) => ({
  albums: [],
  setAlbums: (albums) => set({ albums }),
  addAlbum: (album) => set((state) => ({ albums: [...state.albums, album] })),
  updateAlbum: (id, updates) =>
    set((state) => ({
      albums: state.albums.map((a) => (a.id === id ? { ...a, ...updates } : a)),
    })),
  removeAlbum: (id) =>
    set((state) => ({
      albums: state.albums.filter((a) => a.id !== id),
    })),
}));

interface EnhancementsState {
  jobs: EnhancementJob[];
  subscriptionStatus: SubscriptionStatus | null;
  setJobs: (jobs: EnhancementJob[]) => void;
  addJob: (job: EnhancementJob) => void;
  updateJob: (id: string, updates: Partial<EnhancementJob>) => void;
  setSubscriptionStatus: (status: SubscriptionStatus) => void;
}

export const useEnhancementsStore = create<EnhancementsState>((set) => ({
  jobs: [],
  subscriptionStatus: null,
  setJobs: (jobs) => set({ jobs }),
  addJob: (job) => set((state) => ({ jobs: [job, ...state.jobs] })),
  updateJob: (id, updates) =>
    set((state) => ({
      jobs: state.jobs.map((j) => (j.id === id ? { ...j, ...updates } : j)),
    })),
  setSubscriptionStatus: (status) => set({ subscriptionStatus: status }),
}));

interface UIState {
  sidebarOpen: boolean;
  viewMode: 'grid' | 'list';
  searchQuery: string;
  toggleSidebar: () => void;
  setViewMode: (mode: 'grid' | 'list') => void;
  setSearchQuery: (query: string) => void;
}

export const useUIStore = create<UIState>((set) => ({
  sidebarOpen: true,
  viewMode: 'grid',
  searchQuery: '',
  toggleSidebar: () => set((state) => ({ sidebarOpen: !state.sidebarOpen })),
  setViewMode: (mode) => set({ viewMode: mode }),
  setSearchQuery: (query) => set({ searchQuery: query }),
}));
