import apiClient from "./api";

export const fetchProjects = () => apiClient.get("/projects");
export const fetchProjectById = (projectId) => apiClient.get(`/projects/${projectId}`);
export const createProject = (data) => apiClient.post("/projects", data);
export const updateProject = (projectId, data) => apiClient.put(`/projects/${projectId}`, data);
export const updateProjectStatus = (projectId, status) =>
  apiClient.patch(`/projects/${projectId}/status`, JSON.stringify(status));
export const deleteProject = (projectId) => apiClient.delete(`/projects/${projectId}`);
export const getFeaturesByProject = (projectId) => apiClient.get(`/projects/${projectId}/features`);
