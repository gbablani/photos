// User types
export interface User {
  id: string;
  email: string;
  displayName: string;
  profilePictureUrl?: string;
  subscriptionTier: SubscriptionTier;
  freeEnhancementsRemaining: number;
  enhancementCredits: number;
  subscriptionExpiresAt?: string;
  googlePhotosConnected: boolean;
  oneDriveConnected: boolean;
  autoSyncEnabled: boolean;
}

export type SubscriptionTier = 'Free' | 'PayAsYouGo' | 'Premium';

export type AuthProvider = 'Microsoft' | 'Google' | 'Facebook';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
  expiresAt: string;
}

// Photo types
export interface Photo {
  id: string;
  originalFileName: string;
  blobUrl: string;
  thumbnailUrl?: string;
  fileSize: number;
  width: number;
  height: number;
  dateTaken?: string;
  location?: string;
  description?: string;
  tags?: string;
  isBlackAndWhite: boolean;
  source: PhotoSource;
  isEnhanced: boolean;
  enhancementType?: EnhancementType;
  createdAt: string;
}

export type PhotoSource = 'Upload' | 'GooglePhotos' | 'OneDrive' | 'Dropbox' | 'ICloud';
export type EnhancementType = 'Colorize' | 'Restore' | 'Upscale' | 'LightingCorrection';

export interface PhotoListResponse {
  photos: Photo[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface PhotoSearch {
  query?: string;
  personName?: string;
  startDate?: string;
  endDate?: string;
  source?: PhotoSource;
  isEnhanced?: boolean;
  page?: number;
  pageSize?: number;
}

// Video types
export interface Video {
  id: string;
  originalFileName: string;
  blobUrl: string;
  thumbnailUrl?: string;
  fileSize: number;
  width: number;
  height: number;
  durationSeconds: number;
  dateTaken?: string;
  description?: string;
  source: VideoSource;
  isGenerated: boolean;
  generationType?: VideoGenerationType;
  isEnhanced: boolean;
  createdAt: string;
}

export type VideoSource = 'Upload' | 'GooglePhotos' | 'OneDrive' | 'Generated';
export type VideoGenerationType = 'SinglePhotoAnimation' | 'MultiPhotoMontage' | 'PersonAdded' | 'Extended';

// Album types
export interface Album {
  id: string;
  name: string;
  description?: string;
  coverPhotoUrl?: string;
  photoCount: number;
  createdAt: string;
}

export interface AlbumWithPhotos extends Album {
  photos: Photo[];
}

// Enhancement types
export interface EnhancementJob {
  id: string;
  jobType: JobType;
  status: JobStatus;
  progressPercent: number;
  statusMessage?: string;
  sourcePhotoId?: string;
  sourceVideoId?: string;
  resultPhotoId?: string;
  resultVideoId?: string;
  errorMessage?: string;
  createdAt: string;
  completedAt?: string;
}

export type JobType =
  | 'Colorize'
  | 'RestoreQuality'
  | 'Upscale'
  | 'LightingCorrection'
  | 'SinglePhotoAnimation'
  | 'MultiPhotoMontage'
  | 'AddPersonToVideo'
  | 'ExtendVideo'
  | 'VideoUpscale';

export type JobStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed' | 'Cancelled';

export type AnimationStyle = 'KenBurns' | 'Parallax' | 'SlowZoom' | 'CrossFade';

export interface SubscriptionStatus {
  tier: SubscriptionTier;
  freeEnhancementsRemaining: number;
  enhancementCredits: number;
  expiresAt?: string;
  canEnhance: boolean;
}

// External integration types
export interface ExternalPhoto {
  externalId: string;
  fileName: string;
  thumbnailUrl?: string;
  dateTaken?: string;
  fileSize?: number;
}

export interface ExternalPhotosResponse {
  photos: ExternalPhoto[];
  nextPageToken?: string;
}
