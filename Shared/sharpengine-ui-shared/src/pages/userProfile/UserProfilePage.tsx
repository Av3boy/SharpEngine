import { ProfileHeader } from "./components/ProfileHeader";
import { TabNavigation } from "./components/TabNavigation";
import { AchievementsTab } from "./components/AchievementsTab";
import { AssetsTab } from "./components/AssetsTab";
import { SettingsTab } from "./components/SettingsTab";
import { useState } from "react";

export default function UserProfilePage() {
  const [activeTab, setActiveTab] = useState('achievements');

  return (
    <div className="min-h-screen bg-gray-50">
      <ProfileHeader />
      <TabNavigation activeTab={activeTab} onTabChange={setActiveTab} />
      
      <div className="py-8">
        {activeTab === 'achievements' && <AchievementsTab />}
        {activeTab === 'assets' && <AssetsTab />}
        {activeTab === 'settings' && <SettingsTab />}
      </div>
    </div>
  );
}