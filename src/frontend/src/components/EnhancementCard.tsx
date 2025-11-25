import React from 'react';
import type { EnhancementJob } from '../types';

interface EnhancementCardProps {
  job: EnhancementJob;
  onViewResult?: () => void;
}

export const EnhancementCard: React.FC<EnhancementCardProps> = ({ job, onViewResult }) => {
  const getJobTypeLabel = (type: EnhancementJob['jobType']) => {
    const labels: Record<EnhancementJob['jobType'], string> = {
      Colorize: '🎨 Colorize',
      RestoreQuality: '🔧 Restore Quality',
      Upscale: '📐 Upscale',
      LightingCorrection: '💡 Lighting Correction',
      SinglePhotoAnimation: '🎬 Photo Animation',
      MultiPhotoMontage: '🎞️ Photo Montage',
      AddPersonToVideo: '👤 Add Person to Video',
      ExtendVideo: '⏱️ Extend Video',
      VideoUpscale: '📹 Video Upscale',
    };
    return labels[type] || type;
  };

  const getStatusIcon = (status: EnhancementJob['status']) => {
    switch (status) {
      case 'Pending':
        return '⏳';
      case 'Processing':
        return '🔄';
      case 'Completed':
        return '✅';
      case 'Failed':
        return '❌';
      case 'Cancelled':
        return '🚫';
      default:
        return '❓';
    }
  };

  return (
    <div className={`enhancement-card status-${job.status.toLowerCase()}`}>
      <div className="card-header">
        <span className="job-type">{getJobTypeLabel(job.jobType)}</span>
        <span className="job-status">
          {getStatusIcon(job.status)} {job.status}
        </span>
      </div>

      {job.status === 'Processing' && (
        <div className="progress-container">
          <div className="progress-bar">
            <div className="progress-fill" style={{ width: `${job.progressPercent}%` }} />
          </div>
          <span className="progress-text">{job.progressPercent}%</span>
          {job.statusMessage && <p className="status-message">{job.statusMessage}</p>}
        </div>
      )}

      {job.status === 'Failed' && job.errorMessage && (
        <p className="error-message">{job.errorMessage}</p>
      )}

      <div className="card-footer">
        <span className="job-date">{new Date(job.createdAt).toLocaleString()}</span>
        {job.status === 'Completed' && onViewResult && (
          <button className="view-result-btn" onClick={onViewResult}>
            View Result
          </button>
        )}
      </div>
    </div>
  );
};

interface EnhancementOptionsProps {
  onColorize: () => void;
  onRestore: () => void;
  onAnimate: () => void;
  onMontage: () => void;
  canEnhance: boolean;
  remainingCredits: number;
}

export const EnhancementOptions: React.FC<EnhancementOptionsProps> = ({
  onColorize,
  onRestore,
  onAnimate,
  onMontage,
  canEnhance,
  remainingCredits,
}) => {
  return (
    <div className="enhancement-options">
      <div className="options-header">
        <h3>✨ Enhancement Tools</h3>
        {!canEnhance && (
          <div className="upgrade-notice">
            <p>No credits remaining.</p>
            <a href="/upgrade">Upgrade to continue enhancing</a>
          </div>
        )}
        {canEnhance && remainingCredits > 0 && (
          <p className="credits-info">{remainingCredits} enhancements available</p>
        )}
      </div>

      <div className="options-grid">
        <button className="option-card" onClick={onColorize} disabled={!canEnhance}>
          <div className="option-icon">🎨</div>
          <h4>Colorize B&W</h4>
          <p>Transform black & white photos into vibrant color</p>
        </button>

        <button className="option-card" onClick={onRestore} disabled={!canEnhance}>
          <div className="option-icon">🔧</div>
          <h4>Restore Quality</h4>
          <p>Enhance faded or low-resolution images</p>
        </button>

        <button className="option-card" onClick={onAnimate} disabled={!canEnhance}>
          <div className="option-icon">🎬</div>
          <h4>Animate Photo</h4>
          <p>Bring still photos to life with motion effects</p>
        </button>

        <button className="option-card" onClick={onMontage} disabled={!canEnhance}>
          <div className="option-icon">🎞️</div>
          <h4>Create Montage</h4>
          <p>Combine multiple photos into a video slideshow</p>
        </button>
      </div>
    </div>
  );
};
