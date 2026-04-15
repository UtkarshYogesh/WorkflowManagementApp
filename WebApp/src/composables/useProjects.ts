import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchProjects,
  createProject,
  deleteProject,
  fetchProjectById,
} from "../services/projectApi";

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: async () => (await fetchProjects()).data,
  });
}

export function useProject(projectId: string) {
  return useQuery({
    queryKey: ["project", projectId],
    queryFn: async () => (await fetchProjectById(projectId)).data,
    enabled: !!projectId,
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createProject,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["projects"] }),
  });
}

export function useDeleteProject() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteProject,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["projects"] }),
  });
}
