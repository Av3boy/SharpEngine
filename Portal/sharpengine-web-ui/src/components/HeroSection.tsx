import { useEffect, useRef, useState } from 'react';

export function HeroSection() {
  const parallaxRef = useRef<HTMLDivElement>(null);
  const videoRef = useRef<HTMLVideoElement>(null);
  const [videoIndex, setVideoIndex] = useState(0);

  const videos = [
    'https://videos.pexels.com/video-files/1110140/1110140-uhd_2560_1440_30fps.mp4',
    'https://videos.pexels.com/video-files/3151482/3151482-uhd_2732_1440_24fps.mp4',
    'https://videos.pexels.com/video-files/7170782/7170782-uhd_2732_1440_25fps.mp4',
    'https://videos.pexels.com/video-files/17114954/17114954-uhd_2560_1440_30fps.mp4',
  ];

  useEffect(() => {
    const handleScroll = () => {
      if (parallaxRef.current) {
        const scrolled = window.scrollY;
        parallaxRef.current.style.transform = `translateY(${scrolled * 0.5}px)`;
      }
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    const el = videoRef.current;
    if (!el) return;

    const handleVideoEnd = () => {
      setVideoIndex((prevIndex) => (prevIndex + 1) % videos.length);
    };

    const handleLoadedData = () => {
      // Ensure autoplay resumes when source changes
      const playPromise = el.play();
      if (playPromise && typeof playPromise.then === 'function') {
        playPromise.catch(() => {
          // If autoplay is blocked for some reason, try again on user scroll
          const tryPlay = () => {
            el.play().finally(() => {
              window.removeEventListener('scroll', tryPlay);
            });
          };
          window.addEventListener('scroll', tryPlay, { once: true });
        });
      }
    };

    el.addEventListener('ended', handleVideoEnd);
    el.addEventListener('loadeddata', handleLoadedData);
    return () => {
      el.removeEventListener('ended', handleVideoEnd);
      el.removeEventListener('loadeddata', handleLoadedData);
    };
  }, [videos.length]);

  return (
    <section className="relative h-screen flex items-center justify-center overflow-hidden">
      {/* Parallax Video Background */}
      <div 
        ref={parallaxRef}
        className="absolute inset-0 w-full h-[120%] -top-[10%]"
      >
        <video
          ref={videoRef}
          autoPlay
          muted
          playsInline
          className="w-full h-full object-cover"
          src={videos[videoIndex]}
        />
        {/* Dark overlay */}
        <div className="absolute inset-0 bg-black/60"></div>
      </div>

      {/* Content */}
      <div className="relative z-10 text-center px-6">
        <h1 className="text-white text-7xl mb-6">
          SharpEngine
        </h1>
        <p className="text-white/90 text-2xl">
          No Magic Behind the Scenes 🧙‍♂️✨
        </p>
      </div>

      {/* Scroll indicator */}
      <div className="absolute bottom-8 left-1/2 -translate-x-1/2 z-10">
        <div className="w-6 h-10 border-2 border-white/50 rounded-full flex justify-center p-2">
          <div className="w-1 h-2 bg-white/50 rounded-full animate-bounce"></div>
        </div>
      </div>
    </section>
  );
}
