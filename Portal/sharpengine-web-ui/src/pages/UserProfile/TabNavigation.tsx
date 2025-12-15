interface TabNavigationProps {
  activeTab: string;
  onTabChange: (tab: string) => void;
}

export function TabNavigation({ activeTab, onTabChange }: TabNavigationProps) {
  const tabs = [
    { id: 'achievements', label: 'Achievements' },
    { id: 'assets', label: 'Published Assets' },
    { id: 'settings', label: 'Settings' },
  ];

  return (
    <div className="border-b border-gray-200">
      <div className="max-w-6xl mx-auto">
        <nav className="flex gap-8 px-8">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => onTabChange(tab.id)}
              className={`py-4 border-b-2 transition-colors ${
                activeTab === tab.id
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </nav>
      </div>
    </div>
  );
}
