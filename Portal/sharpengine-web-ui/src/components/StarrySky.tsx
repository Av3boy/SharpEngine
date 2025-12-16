import { useEffect, useRef } from 'react';

interface StarrySkyProps {
  className?: string;
  starCount?: number;
  minDurationMs?: number;
  maxDurationMs?: number;
  "aria-hidden"?: boolean | "true" | "false";
}

export default function StarrySky({
  className = 'sky',
  starCount = 300,
  minDurationMs = 2000,
  maxDurationMs = 6000,
  "aria-hidden": ariaHidden = true,
}: StarrySkyProps) {
  const containerRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const container = containerRef.current;
    if (!container) return;

    const random = (min: number, max: number) => Math.random() * (max - min) + min;

    const placeStar = (star: HTMLDivElement) => {
      star.style.left = Math.random() * 100 + "%";
      star.style.top = Math.random() * 100 + "%";
    };

    const sparkle = (star: HTMLDivElement) => {
      const duration = random(minDurationMs, maxDurationMs);
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
    for (let i = 0; i < starCount; i++) {
      const star = document.createElement("div");
      star.className = "star";
      placeStar(star);
      container.appendChild(star);
      stars.push(star);
      setTimeout(() => sparkle(star), random(0, maxDurationMs));
    }

    return () => {
      stars.forEach((s) => s.remove());
    };
  }, [starCount, minDurationMs, maxDurationMs]);

  return <div ref={containerRef} className={className} aria-hidden={ariaHidden} />;
}
