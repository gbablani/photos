import React from 'react';
import type { Photo } from '../types';
import { usePhotosStore } from '../store';

interface PhotoGridProps {
  photos: Photo[];
  onPhotoClick: (photo: Photo) => void;
  selectable?: boolean;
}

export const PhotoGrid: React.FC<PhotoGridProps> = ({ photos, onPhotoClick, selectable = false }) => {
  const { selectedPhotos, toggleSelectPhoto } = usePhotosStore();

  const handleClick = (photo: Photo, e: React.MouseEvent) => {
    if (selectable && e.ctrlKey) {
      e.preventDefault();
      toggleSelectPhoto(photo.id);
    } else {
      onPhotoClick(photo);
    }
  };

  if (photos.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📷</div>
        <h3>No photos yet</h3>
        <p>Upload your first photo or connect a service to get started</p>
      </div>
    );
  }

  return (
    <div className="photo-grid">
      {photos.map((photo) => (
        <div
          key={photo.id}
          className={`photo-card ${selectedPhotos.includes(photo.id) ? 'selected' : ''}`}
          onClick={(e) => handleClick(photo, e)}
        >
          {selectable && (
            <div className="photo-checkbox">
              <input
                type="checkbox"
                checked={selectedPhotos.includes(photo.id)}
                onChange={() => toggleSelectPhoto(photo.id)}
                onClick={(e) => e.stopPropagation()}
              />
            </div>
          )}

          <div className="photo-image-container">
            <img
              src={photo.thumbnailUrl || photo.blobUrl}
              alt={photo.originalFileName}
              loading="lazy"
            />
            {photo.isBlackAndWhite && <span className="badge bw-badge">B&W</span>}
            {photo.isEnhanced && <span className="badge enhanced-badge">✨ Enhanced</span>}
          </div>

          <div className="photo-info">
            <span className="photo-name">{photo.originalFileName}</span>
            {photo.dateTaken && (
              <span className="photo-date">{new Date(photo.dateTaken).toLocaleDateString()}</span>
            )}
          </div>

          <div className="photo-source">
            {photo.source === 'GooglePhotos' && <span title="Google Photos">🔵</span>}
            {photo.source === 'OneDrive' && <span title="OneDrive">🔷</span>}
            {photo.source === 'Upload' && <span title="Uploaded">📤</span>}
          </div>
        </div>
      ))}
    </div>
  );
};

interface PhotoViewerProps {
  photo: Photo;
  onClose: () => void;
  onColorize: () => void;
  onRestore: () => void;
  onAnimate: () => void;
  onDelete: () => void;
}

export const PhotoViewer: React.FC<PhotoViewerProps> = ({
  photo,
  onClose,
  onColorize,
  onRestore,
  onAnimate,
  onDelete,
}) => {
  return (
    <div className="photo-viewer-overlay" onClick={onClose}>
      <div className="photo-viewer" onClick={(e) => e.stopPropagation()}>
        <button className="close-btn" onClick={onClose}>
          ✕
        </button>

        <div className="viewer-content">
          <div className="viewer-image">
            <img src={photo.blobUrl} alt={photo.originalFileName} />
          </div>

          <div className="viewer-sidebar">
            <h2>{photo.originalFileName}</h2>

            <div className="photo-details">
              {photo.dateTaken && (
                <p>
                  <strong>Date:</strong> {new Date(photo.dateTaken).toLocaleDateString()}
                </p>
              )}
              {photo.location && (
                <p>
                  <strong>Location:</strong> {photo.location}
                </p>
              )}
              <p>
                <strong>Size:</strong> {(photo.fileSize / 1024 / 1024).toFixed(2)} MB
              </p>
              <p>
                <strong>Dimensions:</strong> {photo.width} × {photo.height}
              </p>
              {photo.description && (
                <p>
                  <strong>Description:</strong> {photo.description}
                </p>
              )}
            </div>

            <div className="enhancement-actions">
              <h3>✨ Enhance Photo</h3>

              {photo.isBlackAndWhite && (
                <button className="action-btn colorize" onClick={onColorize}>
                  🎨 Colorize
                </button>
              )}

              <button className="action-btn restore" onClick={onRestore}>
                🔧 Restore Quality
              </button>

              <button className="action-btn animate" onClick={onAnimate}>
                🎬 Create Animation
              </button>
            </div>

            <div className="photo-actions">
              <button className="action-btn download">📥 Download</button>
              <button className="action-btn share">📤 Share</button>
              <button className="action-btn delete" onClick={onDelete}>
                🗑️ Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
