
// import LogoutIcon from '@mui/icons-material/Logout';
import { User } from "@auth0/auth0-react";

export type HeaderLoginProps = {
  onProfileClicked: () => void;
  loginWithRedirect: () => void | Promise<void>;
  logout: () => void | Promise<void>;
  isAuthenticated: boolean;
  user?: User;
  useWithText?: boolean;
};

export function HeaderLogin({onProfileClicked, loginWithRedirect, logout, isAuthenticated, user, useWithText}: HeaderLoginProps) {
  return (
    <div className="flex items-center gap-4">
      {isAuthenticated ? (
        <>
          <button
            className="flex items-center gap-2 px-4 py-2 bg-white/10 hover:bg-white/20 rounded-lg transition-colors"
            onClick={onProfileClicked}
          >
            {/*<UserIcon className={`w-5 h-5 ${useWithText ? "text-white" : ""}`} />*/}
            <span className={` ${useWithText ? "text-white" : ""}`}>Profile</span>
          </button>
          <button onClick={() => logout()}>Logout</button>
        </>
      ) : (
        <button
          className={`px-6 py-2 hover:text-blue-400 transition-colors ${useWithText ? "text-white" : ""}`}
          onClick={() => loginWithRedirect()}
        >
          Login
        </button>
      )}
    </div>
  );
}