import { Header } from './components/Header';
import { HeroSection } from './components/HeroSection';
import { ContentSection } from './components/ContentSection';

export default function App() {
  return (
    <div className="bg-black min-h-screen">
      <Header />
      <HeroSection />
      <ContentSection />
    </div>
  );
}
