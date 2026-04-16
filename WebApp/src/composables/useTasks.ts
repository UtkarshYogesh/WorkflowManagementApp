import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchTasks,
  fetchAllTasks,
  createTask,
  deleteTask,
  updateTaskStatus,
  fetchTaskById,
} from "../services/taskApi";

export function useTasks(backlogId?: string) {
  return useQuery({
    queryKey: ["tasks", backlogId ?? "all"],
    queryFn: async () => {
      if (backlogId) {
        return (await fetchTasks(backlogId)).data;
      }

      return (await fetchAllTasks()).data;
    },
    enabled: true,
  });
}

export function useCreateTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ backlogId, data }: { backlogId: string; data: any }) => createTask(backlogId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks", variables.backlogId] });
    },
  });
}

export function useDeleteTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (taskId: string) => deleteTask(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });
}

export function useUpdateTaskStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: string }) => updateTaskStatus(taskId, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });
}

export function useTask(taskId: string) {
  return useQuery({
    queryKey: ["task", taskId],
    queryFn: async () => (await fetchTaskById(taskId)).data,
    enabled: !!taskId,
  });
}
