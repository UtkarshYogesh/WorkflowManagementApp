import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchProjects,
  createProject,
  deleteProject,
  fetchProjectById,
  updateProjectStatus,
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

export function useUpdateProjectStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ projectId, status }: { projectId: string; status: string }) =>
      updateProjectStatus(projectId, status),
    onSuccess: (response, variables) => {
      queryClient.setQueryData(["project", variables.projectId], response.data);
      queryClient.setQueriesData({ queryKey: ["projects"] }, (oldData: any) => {
        if (!Array.isArray(oldData)) return oldData;

        return oldData.map((project: any) =>
          project.projectId === variables.projectId ? { ...project, ...response.data } : project
        );
      });

      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["projects"] }),
        queryClient.invalidateQueries({ queryKey: ["project", variables.projectId] }),
      ]);
    },
  });
}
