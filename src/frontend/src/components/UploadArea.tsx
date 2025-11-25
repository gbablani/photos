import React, { useCallback, useState } from 'react';

interface UploadAreaProps {
  onUpload: (files: File[]) => void;
  multiple?: boolean;
  accept?: string;
}

export const UploadArea: React.FC<UploadAreaProps> = ({
  onUpload,
  multiple = true,
  accept = 'image/*',
}) => {
  const [isDragging, setIsDragging] = useState(false);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
  }, []);

  const handleDrop = useCallback(
    (e: React.DragEvent) => {
      e.preventDefault();
      setIsDragging(false);

      const files = Array.from(e.dataTransfer.files).filter((file) =>
        file.type.startsWith('image/')
      );

      if (files.length > 0) {
        onUpload(files);
      }
    },
    [onUpload]
  );

  const handleFileSelect = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files ? Array.from(e.target.files) : [];
      if (files.length > 0) {
        onUpload(files);
      }
      e.target.value = ''; // Reset to allow re-selecting same file
    },
    [onUpload]
  );

  return (
    <div
      className={`upload-area ${isDragging ? 'dragging' : ''}`}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      <div className="upload-content">
        <div className="upload-icon">📤</div>
        <h3>Drag & Drop Photos Here</h3>
        <p>or</p>
        <label className="upload-btn">
          Browse Files
          <input
            type="file"
            accept={accept}
            multiple={multiple}
            onChange={handleFileSelect}
            hidden
          />
        </label>
        <p className="upload-hint">Supports JPEG, PNG, GIF, WebP, TIFF</p>
      </div>
    </div>
  );
};

interface UploadProgressProps {
  fileName: string;
  progress: number;
  status: 'uploading' | 'complete' | 'error';
  onCancel?: () => void;
}

export const UploadProgress: React.FC<UploadProgressProps> = ({
  fileName,
  progress,
  status,
  onCancel,
}) => {
  return (
    <div className={`upload-progress ${status}`}>
      <div className="upload-file-info">
        <span className="file-name">{fileName}</span>
        {status === 'uploading' && onCancel && (
          <button className="cancel-btn" onClick={onCancel}>
            ✕
          </button>
        )}
      </div>
      <div className="progress-bar">
        <div className="progress-fill" style={{ width: `${progress}%` }} />
      </div>
      <span className="progress-text">
        {status === 'uploading' && `${progress}%`}
        {status === 'complete' && '✓ Complete'}
        {status === 'error' && '✕ Failed'}
      </span>
    </div>
  );
};
