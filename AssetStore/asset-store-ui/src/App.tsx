import { Header } from "./components/Header";
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { HomePage } from "./pages/HomePage";

export default function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50">
        <Header />
        <Routes>
          <Route path="/" element={<HomePage />} />
        </Routes>
      </div>
    </Router>
  );
}
