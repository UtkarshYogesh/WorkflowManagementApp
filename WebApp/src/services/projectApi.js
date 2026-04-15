import apiClient from "./api";

export const fetchProjects = () => apiClient.get("/projects");
export const fetchProjectById = (projectId) => apiClient.get(`/projects/${projectId}`);
export const createProject = (data) => apiClient.post("/projects", data);
export const deleteProject = (projectId) => apiClient.delete(`/projects/${projectId}`);
export const getFeaturesByProject = (projectId) => apiClient.get(`/projects/${projectId}/features`);
