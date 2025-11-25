import React, { useState } from 'react';
import { Layout } from '../components/Layout';
import { useAuthStore, useEnhancementsStore } from '../store';
import { enhancementsApi } from '../api';

export const UpgradePage: React.FC = () => {
  const { user, updateUser } = useAuthStore();
  const { subscriptionStatus, setSubscriptionStatus } = useEnhancementsStore();
  const [purchasing, setPurchasing] = useState(false);

  const handlePurchaseCredits = async (amount: number) => {
    setPurchasing(true);
    try {
      await enhancementsApi.purchaseCredits(amount);
      // Refresh subscription status
      const status = await enhancementsApi.getSubscriptionStatus();
      setSubscriptionStatus(status);
      if (user) {
        updateUser({ enhancementCredits: status.enhancementCredits });
      }
      alert(`Successfully purchased ${amount} credits!`);
    } catch (error) {
      console.error('Purchase failed:', error);
      alert('Purchase failed. Please try again.');
    } finally {
      setPurchasing(false);
    }
  };

  const handleSubscribe = async () => {
    setPurchasing(true);
    try {
      await enhancementsApi.subscribe('Premium');
      const status = await enhancementsApi.getSubscriptionStatus();
      setSubscriptionStatus(status);
      if (user) {
        updateUser({ subscriptionTier: 'Premium', subscriptionExpiresAt: status.expiresAt });
      }
      alert('Successfully subscribed to Premium!');
    } catch (error) {
      console.error('Subscription failed:', error);
      alert('Subscription failed. Please try again.');
    } finally {
      setPurchasing(false);
    }
  };

  return (
    <Layout>
      <div className="upgrade-page">
        <div className="page-header">
          <h2>💎 Upgrade Your Plan</h2>
          <p>Get more enhancements and bring all your memories to life</p>
        </div>

        {/* Current Status */}
        {subscriptionStatus && (
          <div className="current-status">
            <h3>Current Plan: {subscriptionStatus.tier}</h3>
            {subscriptionStatus.tier !== 'Premium' && (
              <p>
                {subscriptionStatus.freeEnhancementsRemaining} free enhancements +{' '}
                {subscriptionStatus.enhancementCredits} credits remaining
              </p>
            )}
            {subscriptionStatus.tier === 'Premium' && subscriptionStatus.expiresAt && (
              <p>Expires: {new Date(subscriptionStatus.expiresAt).toLocaleDateString()}</p>
            )}
          </div>
        )}

        {/* Credit Packs */}
        <div className="pricing-section">
          <h3>📦 Credit Packs</h3>
          <p>Pay as you go - buy credits when you need them</p>

          <div className="pricing-grid">
            <div className="pricing-card">
              <div className="credits">10</div>
              <div className="credits-label">credits</div>
              <div className="price">$4.99</div>
              <p className="price-per">$0.50 per enhancement</p>
              <button
                className="btn primary"
                onClick={() => handlePurchaseCredits(10)}
                disabled={purchasing}
              >
                Buy Now
              </button>
            </div>

            <div className="pricing-card popular">
              <div className="badge">Most Popular</div>
              <div className="credits">25</div>
              <div className="credits-label">credits</div>
              <div className="price">$9.99</div>
              <p className="price-per">$0.40 per enhancement</p>
              <button
                className="btn primary"
                onClick={() => handlePurchaseCredits(25)}
                disabled={purchasing}
              >
                Buy Now
              </button>
            </div>

            <div className="pricing-card">
              <div className="credits">50</div>
              <div className="credits-label">credits</div>
              <div className="price">$17.99</div>
              <p className="price-per">$0.36 per enhancement</p>
              <button
                className="btn primary"
                onClick={() => handlePurchaseCredits(50)}
                disabled={purchasing}
              >
                Buy Now
              </button>
            </div>

            <div className="pricing-card best-value">
              <div className="badge">Best Value</div>
              <div className="credits">100</div>
              <div className="credits-label">credits</div>
              <div className="price">$29.99</div>
              <p className="price-per">$0.30 per enhancement</p>
              <button
                className="btn primary"
                onClick={() => handlePurchaseCredits(100)}
                disabled={purchasing}
              >
                Buy Now
              </button>
            </div>
          </div>
        </div>

        {/* Premium Subscription */}
        <div className="pricing-section premium">
          <h3>🌟 Premium Subscription</h3>
          <p>Unlimited enhancements for power users</p>

          <div className="premium-card">
            <div className="premium-header">
              <h4>Premium Monthly</h4>
              <div className="price">
                $14.99<span>/month</span>
              </div>
            </div>
            <ul className="features-list">
              <li>✓ Unlimited photo enhancements</li>
              <li>✓ Unlimited video animations</li>
              <li>✓ Priority processing queue</li>
              <li>✓ Early access to new features</li>
              <li>✓ Premium customer support</li>
            </ul>
            <button
              className="btn primary large"
              onClick={handleSubscribe}
              disabled={purchasing || subscriptionStatus?.tier === 'Premium'}
            >
              {subscriptionStatus?.tier === 'Premium'
                ? 'Already Subscribed'
                : 'Subscribe to Premium'}
            </button>
          </div>
        </div>

        {/* FAQ */}
        <div className="faq-section">
          <h3>Frequently Asked Questions</h3>
          <div className="faq-list">
            <div className="faq-item">
              <h4>What counts as one enhancement?</h4>
              <p>
                Each photo colorization, restoration, or single photo animation uses 1 credit.
                Multi-photo montages and video enhancements use 2 credits.
              </p>
            </div>
            <div className="faq-item">
              <h4>Do credits expire?</h4>
              <p>No, purchased credits never expire and can be used anytime.</p>
            </div>
            <div className="faq-item">
              <h4>Can I cancel my subscription?</h4>
              <p>
                Yes, you can cancel anytime. You'll keep Premium access until the end of your
                billing period.
              </p>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};
