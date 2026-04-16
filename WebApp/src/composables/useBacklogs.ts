import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchBacklogs,
  fetchAllBacklogs,
  fetchBacklogById,
  createBacklog,
  deleteBacklog,
  updateBacklogStatus,
} from "../services/backlogApi";

export function useBacklogs(featureId?: string) {
  return useQuery({
    queryKey: ["backlogs", featureId ?? "all"],
    queryFn: async () => {
      if (featureId) {
        return (await fetchBacklogs(featureId)).data;
      }

      return (await fetchAllBacklogs()).data;
    },
    enabled: true,
  });
}

export function useBacklog(backlogId: string) {
  return useQuery({
    queryKey: ["backlog", backlogId],
    queryFn: async () => (await fetchBacklogById(backlogId)).data,
    enabled: !!backlogId,
  });
}

type CreateBacklogPayload = {
  featureId: string;
  data: { title: string; description: string; assignedToUserId?: string | null };
};

type UpdateBacklogStatusPayload = {
  backlogId: string;
  status: string;
};

export function useCreateBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: CreateBacklogPayload) => createBacklog(variables.featureId, variables.data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["backlogs", variables.featureId] });
    },
  });
}

export function useDeleteBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (backlogId: string) => deleteBacklog(backlogId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["backlogs"] });
    },
  });
}

export function useUpdateBacklogStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateBacklogStatusPayload) => updateBacklogStatus(variables.backlogId, variables.status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["backlogs"] });
      queryClient.invalidateQueries({ queryKey: ["backlog"] });
    },
  });
}
