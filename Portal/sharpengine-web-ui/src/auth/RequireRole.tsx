import { useAuth0, User } from "@auth0/auth0-react";
import { Navigate, Outlet } from "react-router-dom";

export default function RequireRole({ role }: { role: "admin" | "editor" }) {
  const { user } = useAuth0();

  if (!user) {
    return <Navigate to="/403" replace />;
  }

  const authUser = user as User | undefined;
  const roles = (authUser as unknown as { roles?: string[] } | undefined)?.roles ?? [];

  if (!roles.includes(role)) {
    return <Navigate to="/403" replace />;
  }
  
  return <Outlet />;
}