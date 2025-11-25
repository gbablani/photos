import React, { useEffect, useState, useCallback } from 'react';
import { Layout } from '../components/Layout';
import { PhotoGrid, PhotoViewer } from '../components/PhotoGrid';
import { UploadArea } from '../components/UploadArea';
import { usePhotosStore, useEnhancementsStore, useUIStore } from '../store';
import { photosApi, enhancementsApi } from '../api';
import type { Photo } from '../types';

export const PhotosPage: React.FC = () => {
  const { photos, setPhotos, setCurrentPhoto, currentPhoto, addPhoto, removePhoto } = usePhotosStore();
  const { subscriptionStatus, setSubscriptionStatus } = useEnhancementsStore();
  const { viewMode, setViewMode, searchQuery } = useUIStore();
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [showUpload, setShowUpload] = useState(false);

  const loadPhotos = useCallback(async () => {
    try {
      setLoading(true);
      const response = await photosApi.getPhotos({ query: searchQuery || undefined });
      setPhotos(response.photos, response.totalCount, response.page, response.pageSize);
    } catch (error) {
      console.error('Failed to load photos:', error);
    } finally {
      setLoading(false);
    }
  }, [searchQuery, setPhotos]);

  const loadSubscriptionStatus = useCallback(async () => {
    try {
      const status = await enhancementsApi.getSubscriptionStatus();
      setSubscriptionStatus(status);
    } catch (error) {
      console.error('Failed to load subscription status:', error);
    }
  }, [setSubscriptionStatus]);

  useEffect(() => {
    loadPhotos();
    loadSubscriptionStatus();
  }, [loadPhotos, loadSubscriptionStatus]);

  const handleUpload = async (files: File[]) => {
    setUploading(true);
    try {
      for (const file of files) {
        const photo = await photosApi.uploadPhoto(file);
        addPhoto(photo);
      }
      setShowUpload(false);
    } catch (error) {
      console.error('Upload failed:', error);
    } finally {
      setUploading(false);
    }
  };

  const handlePhotoClick = (photo: Photo) => {
    setCurrentPhoto(photo);
  };

  const handleCloseViewer = () => {
    setCurrentPhoto(null);
  };

  const handleColorize = async () => {
    if (!currentPhoto) return;
    try {
      await enhancementsApi.colorizePhoto(currentPhoto.id);
      alert('Colorization job started! Check the Enhancements page for progress.');
    } catch (error) {
      console.error('Failed to start colorization:', error);
      alert('Failed to start colorization. Please try again.');
    }
  };

  const handleRestore = async () => {
    if (!currentPhoto) return;
    try {
      await enhancementsApi.restorePhoto(currentPhoto.id);
      alert('Restoration job started! Check the Enhancements page for progress.');
    } catch (error) {
      console.error('Failed to start restoration:', error);
      alert('Failed to start restoration. Please try again.');
    }
  };

  const handleAnimate = async () => {
    if (!currentPhoto) return;
    try {
      await enhancementsApi.animatePhoto(currentPhoto.id);
      alert('Animation job started! Check the Enhancements page for progress.');
    } catch (error) {
      console.error('Failed to start animation:', error);
      alert('Failed to start animation. Please try again.');
    }
  };

  const handleDelete = async () => {
    if (!currentPhoto) return;
    if (!window.confirm('Are you sure you want to delete this photo?')) return;

    try {
      await photosApi.deletePhoto(currentPhoto.id);
      removePhoto(currentPhoto.id);
      setCurrentPhoto(null);
    } catch (error) {
      console.error('Failed to delete photo:', error);
      alert('Failed to delete photo. Please try again.');
    }
  };

  return (
    <Layout>
      <div className="photos-page">
        <div className="page-header">
          <h2>My Photos</h2>
          <div className="header-actions">
            <button
              className={`view-toggle ${viewMode === 'grid' ? 'active' : ''}`}
              onClick={() => setViewMode('grid')}
            >
              ⊞ Grid
            </button>
            <button
              className={`view-toggle ${viewMode === 'list' ? 'active' : ''}`}
              onClick={() => setViewMode('list')}
            >
              ☰ List
            </button>
            <button className="upload-btn primary" onClick={() => setShowUpload(true)}>
              📤 Upload Photos
            </button>
          </div>
        </div>

        {showUpload && (
          <div className="upload-modal">
            <div className="modal-content">
              <button className="close-btn" onClick={() => setShowUpload(false)}>
                ✕
              </button>
              <UploadArea onUpload={handleUpload} />
              {uploading && <p className="uploading-text">Uploading...</p>}
            </div>
          </div>
        )}

        {loading ? (
          <div className="loading">
            <div className="spinner"></div>
            <p>Loading photos...</p>
          </div>
        ) : (
          <PhotoGrid photos={photos} onPhotoClick={handlePhotoClick} selectable />
        )}

        {currentPhoto && (
          <PhotoViewer
            photo={currentPhoto}
            onClose={handleCloseViewer}
            onColorize={handleColorize}
            onRestore={handleRestore}
            onAnimate={handleAnimate}
            onDelete={handleDelete}
          />
        )}

        {subscriptionStatus && !subscriptionStatus.canEnhance && (
          <div className="upgrade-banner">
            <p>
              🎉 You've used your free enhancements! Upgrade to continue bringing your memories to
              life.
            </p>
            <a href="/upgrade" className="upgrade-btn">
              Upgrade Now
            </a>
          </div>
        )}
      </div>
    </Layout>
  );
};
