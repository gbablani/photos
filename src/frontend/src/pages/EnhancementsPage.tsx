import React, { useEffect, useState, useCallback } from 'react';
import { Layout } from '../components/Layout';
import { EnhancementCard } from '../components/EnhancementCard';
import { useEnhancementsStore } from '../store';
import { enhancementsApi } from '../api';
import type { JobStatus } from '../types';

export const EnhancementsPage: React.FC = () => {
  const { jobs, setJobs, subscriptionStatus, setSubscriptionStatus } = useEnhancementsStore();
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<JobStatus | 'all'>('all');

  const loadJobs = useCallback(async () => {
    try {
      setLoading(true);
      const status = filter === 'all' ? undefined : filter;
      const jobsList = await enhancementsApi.getJobs(status);
      setJobs(jobsList);
    } catch (error) {
      console.error('Failed to load jobs:', error);
    } finally {
      setLoading(false);
    }
  }, [filter, setJobs]);

  const loadSubscriptionStatus = useCallback(async () => {
    try {
      const status = await enhancementsApi.getSubscriptionStatus();
      setSubscriptionStatus(status);
    } catch (error) {
      console.error('Failed to load subscription status:', error);
    }
  }, [setSubscriptionStatus]);

  useEffect(() => {
    loadJobs();
    loadSubscriptionStatus();
  }, [loadJobs, loadSubscriptionStatus]);

  // Poll for processing jobs
  useEffect(() => {
    const processingJobs = jobs.filter((j) => j.status === 'Processing' || j.status === 'Pending');
    if (processingJobs.length > 0) {
      const interval = setInterval(loadJobs, 5000);
      return () => clearInterval(interval);
    }
  }, [jobs, loadJobs]);

  const handleViewResult = (jobId: string) => {
    const job = jobs.find((j) => j.id === jobId);
    if (job?.resultPhotoId) {
      window.location.href = `/photos?view=${job.resultPhotoId}`;
    } else if (job?.resultVideoId) {
      window.location.href = `/videos?view=${job.resultVideoId}`;
    }
  };

  return (
    <Layout>
      <div className="enhancements-page">
        <div className="page-header">
          <h2>✨ Enhancements</h2>
          <div className="header-info">
            {subscriptionStatus && (
              <div className="subscription-summary">
                <span className="tier-badge">{subscriptionStatus.tier}</span>
                {subscriptionStatus.canEnhance ? (
                  <span className="credits-available">
                    {subscriptionStatus.tier === 'Premium'
                      ? 'Unlimited'
                      : `${subscriptionStatus.freeEnhancementsRemaining + subscriptionStatus.enhancementCredits} credits`}
                  </span>
                ) : (
                  <a href="/upgrade" className="upgrade-link">
                    Get More Credits
                  </a>
                )}
              </div>
            )}
          </div>
        </div>

        <div className="filter-tabs">
          <button
            className={`filter-tab ${filter === 'all' ? 'active' : ''}`}
            onClick={() => setFilter('all')}
          >
            All
          </button>
          <button
            className={`filter-tab ${filter === 'Processing' ? 'active' : ''}`}
            onClick={() => setFilter('Processing')}
          >
            🔄 Processing
          </button>
          <button
            className={`filter-tab ${filter === 'Completed' ? 'active' : ''}`}
            onClick={() => setFilter('Completed')}
          >
            ✅ Completed
          </button>
          <button
            className={`filter-tab ${filter === 'Failed' ? 'active' : ''}`}
            onClick={() => setFilter('Failed')}
          >
            ❌ Failed
          </button>
        </div>

        {loading ? (
          <div className="loading">
            <div className="spinner"></div>
            <p>Loading enhancement jobs...</p>
          </div>
        ) : jobs.length === 0 ? (
          <div className="empty-state">
            <div className="empty-icon">✨</div>
            <h3>No enhancement jobs yet</h3>
            <p>
              Select a photo and use enhancement tools to colorize, restore, or animate your
              memories.
            </p>
            <a href="/photos" className="btn primary">
              Go to Photos
            </a>
          </div>
        ) : (
          <div className="jobs-list">
            {jobs.map((job) => (
              <EnhancementCard
                key={job.id}
                job={job}
                onViewResult={job.status === 'Completed' ? () => handleViewResult(job.id) : undefined}
              />
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};
