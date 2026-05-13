import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchFeatures,
  fetchAllFeatures,
  fetchFeatureById,
  createFeature,
  updateFeature,
  deleteFeature,
  updateFeatureStatus,
  assignFeatureToUser,
} from "../services/featureApi";

export function useFeatures(projectId?: string) {
  return useQuery({
    queryKey: ["features", projectId ?? "all"],
    queryFn: async () => {
      if (projectId) {
        return (await fetchFeatures(projectId)).data;
      }

      return (await fetchAllFeatures()).data;
    },
    enabled: true,
  });
}

export function useFeature(featureId: string) {
  return useQuery({
    queryKey: ["feature", featureId],
    queryFn: async () => (await fetchFeatureById(featureId)).data,
    enabled: !!featureId,
  });
}

type CreateFeaturePayload = {
  projectId: string;
  data: { name: string; description: string; priority?: string; assignedToUserId?: string | null };
};

type UpdateFeaturePayload = {
  featureId: string;
  data: { name: string; description: string; priority: string; assignedToUserId?: string | null };
};

type UpdateFeatureStatusPayload = {
  featureId: string;
  status: string;
};

type AssignFeaturePayload = {
  featureId: string;
  userId: string;
};

const updateFeatureCache = (queryClient: ReturnType<typeof useQueryClient>, updatedFeature: any) => {
  if (!updatedFeature?.id) return;

  queryClient.setQueryData(["feature", updatedFeature.id], updatedFeature);
  queryClient.setQueriesData({ queryKey: ["features"] }, (oldData: any) => {
    if (!Array.isArray(oldData)) return oldData;

    return oldData.map((feature: any) =>
      feature.id === updatedFeature.id ? { ...feature, ...updatedFeature } : feature
    );
  });
};

export function useCreateFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: CreateFeaturePayload) => createFeature(variables.projectId, variables.data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["features"] });
      queryClient.invalidateQueries({ queryKey: ["features", variables.projectId] });
    },
  });
}

export function useDeleteFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (featureId: string) => deleteFeature(featureId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["features"] });
    },
  });
}

export function useUpdateFeatureStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateFeatureStatusPayload) => updateFeatureStatus(variables.featureId, variables.status),
    onSuccess: (response, variables) => {
      updateFeatureCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["features"] }),
        queryClient.invalidateQueries({ queryKey: ["feature", variables.featureId] }),
      ]);
    },
  });
}

export function useUpdateFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateFeaturePayload) => updateFeature(variables.featureId, variables.data),
    onSuccess: (response, variables) => {
      updateFeatureCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["features"] }),
        queryClient.invalidateQueries({ queryKey: ["feature", variables.featureId] }),
      ]);
    },
  });
}

export function useAssignFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: AssignFeaturePayload) =>
      assignFeatureToUser(variables.featureId, variables.userId),
    onSuccess: (response, variables) => {
      updateFeatureCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["features"] }),
        queryClient.invalidateQueries({ queryKey: ["feature", variables.featureId] }),
      ]);
    },
  });
}
