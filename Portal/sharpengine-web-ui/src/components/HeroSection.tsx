import { useEffect, useRef, useState } from 'react';

export function HeroSection() {
  const videoRef = useRef<HTMLDivElement>(null);
  const [videoIndex, setVideoIndex] = useState(0);

  const videos = [
    'https://videos.pexels.com/video-files/1110140/1110140-uhd_2560_1440_30fps.mp4',
    'https://videos.pexels.com/video-files/3151482/3151482-uhd_2732_1440_24fps.mp4',
    'https://videos.pexels.com/video-files/7170782/7170782-uhd_2732_1440_25fps.mp4',
    'https://videos.pexels.com/video-files/17114954/17114954-uhd_2560_1440_30fps.mp4',
  ];

  useEffect(() => {
    const handleScroll = () => {
      if (videoRef.current) {
        const scrolled = window.scrollY;
        videoRef.current.style.transform = `translateY(${scrolled * 0.5}px)`;
      }
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    const videoElement = document.querySelector('video');
    if (!videoElement) return;

    const handleVideoEnd = () => {
      setVideoIndex((prevIndex) => (prevIndex + 1) % videos.length);
    };

    videoElement.addEventListener('ended', handleVideoEnd);
    return () => videoElement.removeEventListener('ended', handleVideoEnd);
  }, [videos.length]);

  return (
    <section className="relative h-screen flex items-center justify-center overflow-hidden">
      {/* Parallax Video Background */}
      <div 
        ref={videoRef}
        className="absolute inset-0 w-full h-[120%] -top-[10%]"
      >
        <video
          key={videoIndex}
          autoPlay
          muted
          playsInline
          className="w-full h-full object-cover"
        >
          <source src={videos[videoIndex]} type="video/mp4" />
        </video>
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
