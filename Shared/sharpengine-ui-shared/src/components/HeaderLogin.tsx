import { useAuth0 } from "@auth0/auth0-react";
import { User } from "lucide-react";

export function HeaderLogin({ onProfileClicked }: { onProfileClicked: () => void }) {

  const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0();

  return(
    <div>
      {isAuthenticated ? (
        <>
        <button className="flex items-center gap-2 px-4 py-2 bg-white/10 hover:bg-white/20 rounded-lg transition-colors"
                onClick={onProfileClicked}>
          <User className="w-5 h-5 text-white" />
          <span className="text-white">Profile</span>
        </button>
        <button onClick={() => logout()}>
            Logout
        </button>
        </>
      ) : (
        <button className="px-6 py-2 text-white hover:text-blue-400 transition-colors"
                onClick={() => loginWithRedirect()}>
          Login
        </button>
      )}
    </div>
  );
}