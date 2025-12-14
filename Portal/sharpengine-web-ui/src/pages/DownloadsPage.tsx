// import { } from '@sharpengine-ui/sharpengine-ui/';

import { useEffect, useMemo, useRef, useState } from 'react';
import "./DownloadsPage.tsx.scss";

import DownloadsGrid from './DownloadsGrid';
import type { DownloadsGridProps } from './DownloadsGrid';

// Placeholder types you can extend later for GitHub releases
// Minimal set of properties commonly needed in a downloads page
export interface ReleaseAsset {
  id: number;
  name: string;
  browserDownloadUrl: string;
  size?: number;
  contentType?: string;
}

export interface Release {
  id: number;
  name: string; // human-readable release name
  tag: string; // tag name (e.g., v1.2.3)
  notes?: string; // release notes/body (markdown)
  url: string; // html url to release page
  publishedAt?: string; // ISO date string
  assets: ReleaseAsset[]; // downloadable assets
  prerelease?: boolean;
}

type FetchState = 'idle' | 'loading' | 'error' | 'success';

const GITHUB_API = 'https://api.github.com/repos/Av3boy/SharpEngine/releases';

export default function DownloadsPage() {
  const [releases, setReleases] = useState<Release[]>([]);
  const [state, setState] = useState<FetchState>('idle');
  const [error, setError] = useState<string | null>(null);
  const skyRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    let mounted = true;
    async function load() {
      setState('loading');
      setError(null);
      try {
        const res = await fetch(GITHUB_API, {
          headers: {
            // Unauthenticated requests have low rate limits; add Accept for stable JSON
            Accept: 'application/vnd.github+json',
          },
        });
        if (!res.ok) {
          throw new Error(`GitHub API error: ${res.status} ${res.statusText}`);
        }
        const data = await res.json();
        // Map GitHub API fields to our placeholder Release type
        const mapped: Release[] = (Array.isArray(data) ? data : []).map((r: any) => ({
          id: r.id,
          name: r.name ?? r.tag_name ?? `Release ${r.id}`,
          tag: r.tag_name ?? '',
          notes: r.body ?? '',
          url: r.html_url ?? '',
          publishedAt: r.published_at ?? r.created_at ?? undefined,
          prerelease: !!r.prerelease,
          assets: Array.isArray(r.assets)
            ? r.assets.map((a: any) => ({
                id: a.id,
                name: a.name ?? 'download',
                browserDownloadUrl: a.browser_download_url ?? '',
                size: a.size,
                contentType: a.content_type,
              }))
            : [],
        }));
        if (mounted) {
          setReleases(mapped);
          setState('success');
        }
      } catch (e: any) {
        if (mounted) {
          setError(e?.message ?? 'Failed to load releases');
          setState('error');
        }
      }
    }
    load();
    return () => {
      mounted = false;
    };
  }, []);

  const hasData = useMemo(() => releases.length > 0, [releases]);

  // Starry sky background mounted inside the main content wrapper
  useEffect(() => {
    const container = skyRef.current;
    if (!container) return;

    const STAR_COUNT = 300;
    const MIN_DURATION = 2000;
    const MAX_DURATION = 6000;

    const random = (min: number, max: number) => Math.random() * (max - min) + min;

    const placeStar = (star: HTMLDivElement) => {
      // Position within container bounds using percentages
      star.style.left = Math.random() * 100 + "%";
      star.style.top = Math.random() * 100 + "%";
    };

    const sparkle = (star: HTMLDivElement) => {
      const duration = random(MIN_DURATION, MAX_DURATION);
      star.animate([
        { opacity: 0 },
        { opacity: random(0.6, 1) },
        { opacity: 0 }
      ], {
        duration,
        easing: "ease-in-out"
      }).onfinish = () => {
        placeStar(star);
        sparkle(star);
      };
    };

    const stars: HTMLDivElement[] = [];
    for (let i = 0; i < STAR_COUNT; i++) {
      const star = document.createElement("div");
      star.className = "star";
      placeStar(star);
      container.appendChild(star);
      stars.push(star);
      setTimeout(() => sparkle(star), random(0, MAX_DURATION));
    }

    return () => {
      // Cleanup: remove stars and their animations when component unmounts
      stars.forEach((s) => s.remove());
    };
  }, []);

  return (
    <>
      {/* 
       * Placeholder Starry sky background overlay
       * This is probably way too performance-heavy for production use
       * but serves as a nice visual effect for now.
       * Consider replacing with a static background or optimized canvas effect later.
      */}
      <div ref={skyRef} className="sky" aria-hidden="true" />
      <div className="pt-20 px-6 max-w-5xl mx-auto relative">

        <h1 className="text-white text-4xl font-bold mb-4">Downloads</h1>
        {state === 'loading' && (
          <p className="text-gray-300">Loading releases…</p>
        )}
        {state === 'error' && (
          <div className="text-red-400">{error}</div>
        )}
        {state === 'success' && !hasData && (
          <p className="text-gray-300">No releases found.</p>
        )}

        {hasData && (
          <DownloadsGrid items={releases} />
        )}
      </div>
    </>
  );
}