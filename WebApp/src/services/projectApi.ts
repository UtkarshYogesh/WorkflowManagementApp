import apiClient from "./api";

export const fetchProjects = () => apiClient.get("/projects");
export const fetchProjectById = (projectId: string) => apiClient.get(`/projects/${projectId}`);
export const createProject = (data: any) => apiClient.post("/projects", data);
export const deleteProject = (projectId: string) => apiClient.delete(`/projects/${projectId}`);
export const getFeaturesByProject = (projectId: string) => apiClient.get(`/projects/${projectId}/features`);
