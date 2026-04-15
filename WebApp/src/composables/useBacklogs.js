import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchBacklogs,
  fetchBacklogById,
  createBacklog,
  deleteBacklog,
  updateBacklogStatus,
} from "../services/backlogApi";

export function useBacklogs(featureId) {
  return useQuery({
    queryKey: ["backlogs", featureId],
    queryFn: async () => (await fetchBacklogs(featureId)).data,
    enabled: !!featureId,
  });
}

export function useBacklog(backlogId) {
  return useQuery({
    queryKey: ["backlog", backlogId],
    queryFn: async () => (await fetchBacklogById(backlogId)).data,
    enabled: !!backlogId,
  });
}

export function useCreateBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ featureId, data }) => createBacklog(featureId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(["backlogs", variables.featureId]);
    },
  });
}

export function useDeleteBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (backlogId) => deleteBacklog(backlogId),
    onSuccess: () => {
      queryClient.invalidateQueries(["backlogs"]);
    },
  });
}

export function useUpdateBacklogStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ backlogId, status }) => updateBacklogStatus(backlogId, status),
    onSuccess: () => {
      queryClient.invalidateQueries(["backlogs"]);
      queryClient.invalidateQueries(["backlog"]);
    },
  });
}
